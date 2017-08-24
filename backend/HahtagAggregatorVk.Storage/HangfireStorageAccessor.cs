using System.Collections.Generic;
using Hangfire;
using Hangfire.Storage;
using HashtagAggregator.Service.Contracts;
using HashtagAggregatorVk.Contracts.Settings;
using Microsoft.Extensions.Options;

namespace HahtagAggregatorVk.Storage
{
    public class HangfireStorageAccessor : IStorageAccessor
    {
        private readonly IOptions<HangfireSettings> settings;

        public HangfireStorageAccessor(IOptions<HangfireSettings> settings)
        {
            this.settings = settings;
        }

        public List<RecurringJobDto> GetJobsList()
        {
            return JobStorage.Current.GetConnection().GetRecurringJobs();
        }

        public void CancelRecurringJobs()
        {
            var jobs = JobStorage.Current.GetConnection().GetRecurringJobs();
            foreach (var recurringJob in jobs)
            {
                if (settings.Value != null && recurringJob.Id.StartsWith(settings.Value.ServerName))
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }
        }
    }
}