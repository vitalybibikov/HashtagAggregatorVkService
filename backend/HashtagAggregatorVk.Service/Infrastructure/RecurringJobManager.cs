using System;
using System.Threading.Tasks;
using Hangfire;
using HashtagAggregator.Core.Contracts.Interface.Cqrs.Command;
using HashtagAggregator.Service.Contracts.Jobs;
using HashtagAggregatorVk.Contracts;
using HashtagAggregatorVk.Contracts.Interface.Jobs;
using HashtagAggregatorVk.Contracts.Settings;
using Microsoft.Extensions.Options;

namespace HashtagAggregatorVk.Service.Infrastructure
{
    public class RecurringJobManager : IJobManager
    {
        private readonly IVkJob job;
        private readonly IOptions<HangfireSettings> hangfireOptions;

        public RecurringJobManager(IVkJob job, IOptions<HangfireSettings> hangfireOptions)
        {
            this.job = job;
            this.hangfireOptions = hangfireOptions;
        }

        public ICommandResult AddJob(IJobTask task)
        {
            RecurringJob.AddOrUpdate<IVkJob>(
                task.JobId,
                x => x.Execute((VkJobTask) task),
                Cron.MinuteInterval(task.Interval),
                queue: hangfireOptions.Value.ServerName);
            return new CommandResult {Success = true};
        }

        public ICommandResult DeleteJob(IJobTask task)
        {
            RecurringJob.RemoveIfExists(task.JobId);
            return new CommandResult {Success = true};
        }

        public async Task<ICommandResult> StartNow(IJobTask task)
        {
            return await job.Execute((VkJobTask) task);
        }

        public ICommandResult ReconfigureJob(IJobTask task)
        {
            throw new NotImplementedException();
        }
    }
}