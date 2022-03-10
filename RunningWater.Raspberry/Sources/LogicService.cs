using System;
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
        private readonly IJobScheduler jobScheduler;

#if DEBUG
        private const string DEFAULT_CRON = "*/1 * * * *"; // every 1 minute
#else
        private const string DEFAULT_CRON = "0 12 * * *"; // at 12:00
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="jobScheduler"></param>
        public LogicService(IStorage storage, IJobScheduler jobScheduler)
        {
            this.storage = storage;
            this.jobScheduler = jobScheduler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        [MethodName("job/cron/set")]
        public void SetCron(string cron)
        {
            Cron = cron;

            if (GetState().IsEnabled)
            {
                jobScheduler.AddOrUpdate<IWateringJob>(Cron);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [MethodName("job/state/enable")]
        public void Enable() => jobScheduler.AddOrUpdate<IWateringJob>(Cron);

        /// <summary>
        /// 
        /// </summary>
        [MethodName("job/state/disable")]
        public void Disable() => jobScheduler.RemoveIfExists<IWateringJob>();

        /// <summary>
        /// 
        /// </summary>
        [MethodName("job/state/get")]
        public JobState GetState() => new JobState
        {
            IsEnabled = jobScheduler.IsJobExist<IWateringJob>(),
            Cron = Cron,
        };

        /// <summary>
        /// 
        /// </summary>
        private string Cron
        {
            get => storage.GetValue("cron", DEFAULT_CRON);
            set => storage.SetValue("cron", value);
        }
    }
}
