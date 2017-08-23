using System;
using HashtagAggregator.Service.Contracts.Jobs;
using HashtagAggregator.Service.Contracts.Queues;
using HashtagAggregator.Shared.Common.Infrastructure;

namespace HashtagAggregatorVk.Contracts
{
    public class VkJobTask : IJobTask
    {
        private const string JobIdPattern = "{0}-enqueue-id";

        public HashTagWord Tag { get; }

        public QueueParams Parameters { get; }

        public int Interval { get; }

        public string JobId => String.Format(JobIdPattern, Tag.NoHashTag);

        public VkJobTask(HashTagWord tag, QueueParams parameters, int interval)
        {
            Tag = tag;
            Parameters = parameters;
            Interval = interval;
        }
    }
}
