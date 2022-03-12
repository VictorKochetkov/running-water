using System;
using NCrontab;
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
        [MethodName("cron/write")]
        public void CronWrite(string cron)
        {
            Cron = cron;

            if (StateRead())
            {
                jobScheduler.AddOrUpdate<IWateringJob>(Cron);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [MethodName("cron/read")]
        public string CronRead() => Cron;

        /// <summary>
        /// 
        /// </summary>
        [MethodName("state/write")]
        public void StateWrite(bool enabled)
        {
            if (enabled)
                jobScheduler.AddOrUpdate<IWateringJob>(Cron);
            else
                jobScheduler.RemoveIfExists<IWateringJob>();
        }

        /// <summary>
        /// 
        /// </summary>
        [MethodName("state/read")]
        public bool StateRead() => jobScheduler.IsJobExist<IWateringJob>();

        /// <summary>
        /// 
        /// </summary>
        private string Cron
        {
            get => ValidateCron(storage.GetValue("cron", string.Empty));
            set => storage.SetValue("cron", ValidateCron(value, throwOnError: true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cron"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        private string ValidateCron(string cron, bool throwOnError = false)
        {
            if (CrontabSchedule.TryParse(cron) != null)
                return cron;

            if (throwOnError)
                throw new ArgumentException("Cron string is invalid");

            return DEFAULT_CRON;
        }
    }
}
