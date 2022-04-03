using System;
using System.Linq;
using Hangfire;
using Hangfire.Storage;
using RunningWater.Raspberry.Interfaces;

namespace RunningWater.Raspberry.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class HangfireJobScheduler<TJob> : IJobScheduler<TJob> where TJob : IJob
    {
        /// <inheritdoc/>
        public string Add(DateTimeOffset enqueueAt)
        {
            return BackgroundJob.Schedule<TJob>(job => job.ExecuteAsync(), enqueueAt);
        }

        /// <inheritdoc/>
        public void Remove(DateTimeOffset enqueueAt)
        {
            if (IsJobExist(enqueueAt, out string jobId))
                BackgroundJob.Delete(jobId);
        }

        /// <inheritdoc/>
        public void RemoveAll()
        {
            var jobs = JobStorage.Current.GetMonitoringApi()
                .ScheduledJobs(0, int.MaxValue)
                .Select(pair => pair.Key);

            foreach (var jobId in jobs)
                BackgroundJob.Delete(jobId);
        }

        /// <inheritdoc/>
        public bool IsJobExist(DateTimeOffset enqueueAt, out string jobId)
        {
            jobId = JobStorage.Current.GetMonitoringApi()
                .ScheduledJobs(0, int.MaxValue)
                .SingleOrDefault(pair => pair.Value.EnqueueAt == enqueueAt).Key;

            return !string.IsNullOrEmpty(jobId);
        }
    }
}
