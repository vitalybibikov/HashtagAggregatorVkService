using System.Collections.Generic;
using System.Threading.Tasks;
using HashtagAggregator.Core.Contracts.Interface.Cqrs.Command;
using HashtagAggregator.Core.Entities.VkEntities;
using HashtagAggregator.Service.Contracts.Queues;
using HashtagAggregatorVk.Contracts;
using HashtagAggregatorVk.Contracts.Interface.Queues;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace HashtagAggregatorVk.Service.Infrastructure.Queues
{
    public class VkQueue : IVkQueue
    {
        private readonly IAzureQueueInitializer initializer;

        public VkQueue(IAzureQueueInitializer initializer)
        {
            this.initializer = initializer;
        }

        public async Task<ICommandResult> Enqueue(VkNewsFeed feed)
        {
            var message = JsonConvert.SerializeObject(feed);
            var result = new CloudQueueMessage(message);
            await initializer.Queue.AddMessageAsync(result);
            return new CommandResult
            {
                Success = true
            };
        }
    }
}