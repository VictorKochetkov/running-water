using System;

namespace RunningWater.Raspberry.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TJob"></typeparam>
    public interface IJobScheduler<TJob> where TJob : IJob
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enqueueAt"></param>
        /// <returns></returns>
        string Add(DateTimeOffset enqueueAt);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enqueueAt"></param>
        void Remove(DateTimeOffset enqueueAt);

        /// <summary>
        /// 
        /// </summary>
        void RemoveAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enqueueAt"></param>
        /// <param name="jobId"></param>
        /// <returns></returns>
        bool IsJobExist(DateTimeOffset enqueueAt, out string jobId);
    }
}
