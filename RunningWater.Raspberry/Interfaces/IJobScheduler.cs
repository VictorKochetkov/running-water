namespace RunningWater.Raspberry.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IJobScheduler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="cron"></param>
        void AddOrUpdate<TJob>(string cron) where TJob : IJob;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        void RemoveIfExists<TJob>() where TJob : IJob;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <returns></returns>
        bool IsJobExist<TJob>() where TJob : IJob;
    }
}
