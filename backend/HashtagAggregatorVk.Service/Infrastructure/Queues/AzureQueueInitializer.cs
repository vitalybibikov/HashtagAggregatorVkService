using HashtagAggregator.Service.Contracts.Queues;
using HashtagAggregatorVk.Contracts.Settings;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace HashtagAggregatorVk.Service.Infrastructure.Queues
{
    public class AzureQueueInitializer : IAzureQueueInitializer
    {
        private readonly IOptions<QueueSettings> queueSettings;
        private CloudQueue queue;

        public CloudQueue Queue => queue;

        public AzureQueueInitializer(IOptions<QueueSettings> queueSettings)
        {
            this.queueSettings = queueSettings;
            Initialize();
        }

        private void Initialize()
        {
            var storageAccount = CloudStorageAccount.Parse(queueSettings.Value.StorageConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            queue = queueClient.GetQueueReference(queueSettings.Value.QueueName);
            queue.CreateIfNotExistsAsync();
        }
    }
}