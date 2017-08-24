using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace HashtagAggregatorVk.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://localhost:5011/")
                .UseDefaultServiceProvider(options =>
                    options.ValidateScopes = false)
                .UseApplicationInsights()
                .Build();
    }
}
