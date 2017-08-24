using Autofac;
using HashtagAggregatorVk.Contracts.Interface.Jobs;
using HashtagAggregatorVk.Contracts.Interface.Queues;
using HashtagAggregatorVk.Service.Infrastructure.Jobs;
using HashtagAggregatorVk.Service.Infrastructure.Queues;

namespace HashtagAggregatorVk.Service.Configuration.Modules
{
    public class VkModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<VkJob>().As<IVkJob>();
            builder.RegisterType<VkQueue>().As<IVkQueue>();
        }
    }
}