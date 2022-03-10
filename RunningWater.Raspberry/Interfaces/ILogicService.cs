using System;

namespace RunningWater.Raspberry.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public class JobState
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Cron { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface ILogicService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cron"></param>
        public void SetCron(string cron);

        /// <summary>
        /// 
        /// </summary>
        public void Enable();

        /// <summary>
        /// 
        /// </summary>
        public void Disable();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JobState GetState();
    }
}
