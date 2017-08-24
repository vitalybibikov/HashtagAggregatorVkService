using System.Linq;
using System.Threading.Tasks;
using HashtagAggregator.Core.Contracts.Interface.Cqrs.Command;
using HashtagAggregator.Service.Contracts;
using HashtagAggregator.Service.Contracts.Jobs;
using HashtagAggregator.Service.Contracts.Queues;
using HashtagAggregator.Shared.Common.Infrastructure;
using HashtagAggregatorVk.Contracts;
using HashtagAggregatorVk.Contracts.Settings;
using Microsoft.Extensions.Options;

namespace HashtagAggregatorVk.Service.Infrastructure
{
    public class VkJobBalancer : ISocialJobBalancer
    {
        private readonly IStorageAccessor accessor;
        private readonly IOptions<VkApiSettings> settings;
        private readonly IOptions<AppSettings> appSettings;
        private readonly IOptions<HangfireSettings> hangfireSettings;
        private readonly IJobManager jobManager;

        public VkJobBalancer(IStorageAccessor accessor,
            IOptions<VkApiSettings> settings,
            IOptions<AppSettings> appSettings,
            IOptions<HangfireSettings> hangfireSettings,
            IJobManager jobManager)
        {
            this.accessor = accessor;
            this.settings = settings;
            this.appSettings = appSettings;
            this.hangfireSettings = hangfireSettings;
            this.jobManager = jobManager;
        }

        public async Task<ICommandResult> TryCreateJob(string tag)
        {
            var isAdded = new CommandResult();
            // todo: move to settings or make review interval
            var qParams = new QueueParams(tag, hangfireSettings.Value.ServerName);
            var intervalMonth = 30 * 60 * 24;
            var initTask = new VkJobTask(new HashTagWord(tag), qParams, intervalMonth);

            if (!CheckJobLimitExceeded(initTask))
            {
                await jobManager.StartNow(initTask);
                AddJob(initTask);
                isAdded.Success = true;
                isAdded.Message = "Vk Job created.";
            }
            else
            {
                isAdded.Message = "Job Limit Exceeded";
            }
            return isAdded;
        }

        private void AddJob(IJobTask task)
        {
            var qParams = new QueueParams(task.Tag.NoHashTag, hangfireSettings.Value.ServerName);
            task = new VkJobTask(task.Tag, qParams, settings.Value.VkMessagePublishDelay);
            jobManager.AddJob(task);
        }

        public ICommandResult DeleteJob(string tag)
        {
            var qParams = new QueueParams(tag, hangfireSettings.Value.ServerName);
            var task = new VkJobTask(new HashTagWord(tag), qParams, 0);
            return jobManager.DeleteJob(task);
        }

        private bool CheckJobLimitExceeded(IJobTask task)
        {
            var list = accessor.GetJobsList();
            var isValid = list.Any(x => x.Id.Equals(task.JobId));
            return isValid || list.Count >= appSettings.Value.MaxReccuringJobsSupported;
        }
    }
}