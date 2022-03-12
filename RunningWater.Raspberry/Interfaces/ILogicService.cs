using System;

namespace RunningWater.Raspberry.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILogicService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cron"></param>
        public void CronWrite(string cron);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string CronRead();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        public void StateWrite(bool enabled);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool StateRead();
    }
}
