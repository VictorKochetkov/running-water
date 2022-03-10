using System.Linq;
using Hangfire;
using Hangfire.Storage;
using RunningWater.Raspberry.Interfaces;

namespace RunningWater.Raspberry.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class HangfireJobScheduler : IJobScheduler
    {
        /// <inheritdoc/>
        public void AddOrUpdate<TJob>(string cron) where TJob : IJob
            => RecurringJob.AddOrUpdate<TJob>(GetJobId<TJob>(), job => job.ExecuteAsync(), () => cron);

        /// <inheritdoc/>
        public void RemoveIfExists<TJob>() where TJob : IJob
            => RecurringJob.RemoveIfExists(GetJobId<TJob>());

        /// <inheritdoc/>
        public bool IsJobExist<TJob>() where TJob : IJob
            => JobStorage.Current.GetConnection().GetRecurringJobs().SingleOrDefault() is not null;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <returns></returns>
        private static string GetJobId<TJob>() => typeof(TJob).Name;
    }
}
