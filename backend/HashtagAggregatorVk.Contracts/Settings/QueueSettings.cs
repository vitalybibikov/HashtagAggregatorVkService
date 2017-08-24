namespace HashtagAggregatorVk.Contracts.Settings
{
    public class QueueSettings
    {
        public string StorageConnectionString { get; set; }

        public string DefaultEndpointsProtocol { get; set; }

        public string QueueName { get; set; }

        public string AccountName { get; set; }

        public string AccountKey { get; set; }

        public string QueueEndpoint { get; set; }

        public string EndpointSuffix { get; set; }
    }
}