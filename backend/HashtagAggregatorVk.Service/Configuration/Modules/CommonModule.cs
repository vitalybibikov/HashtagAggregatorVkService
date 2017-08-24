using Autofac;
using HahtagAggregatorVk.Storage;
using HashtagAggregator.Service.Contracts;
using HashtagAggregator.Service.Contracts.Jobs;
using HashtagAggregator.Service.Contracts.Queues;
using HashtagAggregatorVk.Service.Infrastructure;
using HashtagAggregatorVk.Service.Infrastructure.Queues;
using IBackgroundServiceWorker = HashtagAggregatorVk.Contracts.Interface.IBackgroundServiceWorker;

namespace HashtagAggregatorVk.Service.Configuration.Modules
{
    public class CommonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AzureQueueInitializer>().As<IAzureQueueInitializer>();
            builder.RegisterType<RecurringJobManager>().As<IJobManager>();
            builder.RegisterType<BackgroundServiceWorker>().As<IBackgroundServiceWorker>();
            builder.RegisterType<VkJobBalancer>().As<ISocialJobBalancer>();
            builder.RegisterType<HangfireStorageAccessor>().As<IStorageAccessor>().SingleInstance();
        }
    }
}