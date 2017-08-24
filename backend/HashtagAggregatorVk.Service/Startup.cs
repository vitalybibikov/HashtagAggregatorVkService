using System;
using System.Net;
using Autofac;
using Hangfire;
using HashtagAggregator.Service.Contracts;
using HashtagAggregatorVk.Contracts.Settings;
using HashtagAggregatorVk.Service.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace HashtagAggregatorVk.Service
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(Configuration)
                .WriteTo.ApplicationInsightsTraces(
                    Configuration.GetSection("ApplicationInsights:InstrumentationKey").Value)
                .CreateLogger();

            if (env.IsEnvironment("dev"))
            {
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<QueueSettings>(Configuration.GetSection("QueueSettings"));
            services.Configure<VkAuthSettings>(Configuration.GetSection("VkAuthSettings"));
            services.Configure<VkApiSettings>(Configuration.GetSection("VkApiSettings"));
            services.Configure<HangfireSettings>(Configuration.GetSection("HangfireSettings"));

            var connectionString = Configuration.GetSection("AppSettings:ConnectionString").Value;

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddHangfire(config => config.UseSqlServerStorage(connectionString));
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc();

            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()));

            var container = new AutofacModulesConfigurator().Configure(services);
            GlobalConfiguration.Configuration.UseActivator(new AutofacContainerJobActivator(container));
            return container.Resolve<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IStorageAccessor accessor)
        {
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            app.UseExceptionHandler(options =>
            {
                options.Run(
                    async context =>
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                        var ex = context.Features.Get<IExceptionHandlerFeature>();
                        if (ex != null)
                        {
                            var err = $"Error: {ex.Error.Message}{ex.Error.StackTrace}";
                            Log.Error(ex.Error, "Server Error", ex);
                            await context.Response.WriteAsync(err).ConfigureAwait(false);
                        }
                    });
            });

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                ServerName = Configuration.GetSection("HangfireSettings:ServerName").Value,
                Queues = new[] {Configuration.GetSection("HangfireSettings:ServerName").Value}
            });
            app.UseHangfireDashboard();
            if (env.IsEnvironment("dev"))
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("CorsPolicy");

            //app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            //{
            //    Authority = Configuration.GetSection("EndpointSettings:AuthEndpoint").Value,
            //    RequireHttpsMetadata = false, //todo: should be true when enabled https
            //    ApiName = "vkapiservice",
            //    CacheDuration = TimeSpan.FromMinutes(10)
            //});

            accessor.CancelRecurringJobs();
            app.UseMvc();
        }
    }
}