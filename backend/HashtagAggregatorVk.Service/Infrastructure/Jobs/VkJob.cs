using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Hangfire;
using HashtagAggregator.Core.Contracts.Interface.Cqrs.Command;
using HashtagAggregator.Core.Entities.VkEntities;
using HashtagAggregator.Shared.Common.Helpers;
using HashtagAggregator.Shared.Logging;
using HashtagAggregatorVk.Contracts;
using HashtagAggregatorVk.Contracts.Interface.Jobs;
using HashtagAggregatorVk.Contracts.Interface.Queues;
using HashtagAggregatorVk.Contracts.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HashtagAggregatorVk.Service.Infrastructure.Jobs
{
    public class VkJob : IVkJob
    {
        private readonly IVkQueue queue;
        private readonly IOptions<VkApiSettings> settings;
        private readonly IOptions<VkAuthSettings> authVkSettings;
        private readonly ILogger<VkJob> logger;

        public VkJob(IVkQueue queue,
            IOptions<VkApiSettings> settings, 
            IOptions<VkAuthSettings> authVkSettings,
            ILogger<VkJob> logger)
        {
            this.queue = queue;
            this.settings = settings;
            this.authVkSettings = authVkSettings;
            this.logger = logger;
        }

        [AutomaticRetry(Attempts = 1)]
        [Queue("vkserver")]
        public async Task<ICommandResult> Execute(VkJobTask task)
        {
            using (var request = new WebRequestWrapper())
            {
                var query =
                    new VkMessageQuery(settings.Value.MessagesApiUrl,
                        settings.Value.ApiVersion,
                        authVkSettings.Value.ServiceToken)
                    {
                        Query = task.Tag.ToString()
                    };

                var json = await request.LoadJsonAsync(HttpMethod.Get, query.ToString());
                var jObject = JObject.Parse(json).SelectToken("response").ToString();

                if (String.IsNullOrEmpty(jObject))
                {
                    logger.LogError(
                        LoggingEvents.EXCEPTION_GET_VK_MESSAGE,
                        "Failed to get messages by {hashtag} with {error}",
                        task.Tag,
                        jObject);
                    throw new InvalidDataException(jObject);
                }

                var feed = CheckThatObjectCorrect(jObject);
                return await queue.Enqueue(feed);
            }
        }

        private static VkNewsFeed CheckThatObjectCorrect(string jObject)
        {
            var feed = JsonConvert.DeserializeObject<VkNewsFeed>(jObject);
            return feed;
        }
    }
}