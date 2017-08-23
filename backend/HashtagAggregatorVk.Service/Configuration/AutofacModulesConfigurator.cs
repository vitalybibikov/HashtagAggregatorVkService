using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutofacSerilogIntegration;
using HashtagAggregatorVk.Service.Configuration.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace HashtagAggregatorVk.Service.Configuration
{
    public class AutofacModulesConfigurator
    {
        public IContainer Configure(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.RegisterLogger();

            builder.RegisterModule<VkModule>();
            builder.RegisterModule<CommonModule>();

            builder.Populate(services);
            return builder.Build();
        }
    }
}
