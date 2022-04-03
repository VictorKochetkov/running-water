using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RunningWater.Raspberry.Attributes;
using RunningWater.Raspberry.Interfaces;

namespace RunningWater.Raspberry.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class LogicService : ILogicService
    {
        private readonly IStorage storage;
        private readonly IJobScheduler<IWateringJob> jobScheduler;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="jobScheduler"></param>
        public LogicService(IStorage storage, IJobScheduler<IWateringJob> jobScheduler)
        {
            this.storage = storage;
            this.jobScheduler = jobScheduler;

            UpdateJobs(Jobs);
        }

        /// <inheritdoc/>
        [MethodName("job/write")]
        public void JobsWrite(IEnumerable<DateTimeOffset> jobs)
        {
            Jobs = jobs;
            UpdateJobs(Jobs);
        }

        /// <inheritdoc/>
        [MethodName("state/write")]
        public void StateWrite(bool enabled)
        {
            IsEnabled = enabled;
            UpdateJobs(Jobs);
        }

        /// <inheritdoc/>
        [MethodName("job/read")]
        public IEnumerable<DateTimeOffset> JobsRead()
        {
            return Jobs;
        }

        /// <inheritdoc/>
        [MethodName("state/read")]
        public bool StateRead()
        {
            return IsEnabled;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateJobs(IEnumerable<DateTimeOffset> jobs)
        {
            Task.Run(() =>
            {
                lock (typeof(ILogicService))
                {
                    jobScheduler.RemoveAll();

                    if (IsEnabled)
                        foreach (var schedule in jobs)
                            jobScheduler.Add(schedule);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<DateTimeOffset> Jobs
        {
            get => storage.GetValue("jobs", Enumerable.Empty<DateTimeOffset>());
            set => storage.SetValue("jobs", value);
        }

        /// <summary>
        /// 
        /// </summary>
        private bool IsEnabled
        {
            get => storage.GetValue("enabled", false);
            set => storage.SetValue("enabled", value);
        }
    }
}
