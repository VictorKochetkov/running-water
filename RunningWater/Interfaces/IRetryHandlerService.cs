using System;
using System.Threading.Tasks;

namespace RunningWater.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public enum RetryPolicy
    {
        Infinite
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IRetryHandlerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="policy"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        Task ExecuteAsync(Func<Task> action, RetryPolicy policy = RetryPolicy.Infinite, TimeSpan? delay = null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="action"></param>
        /// <param name="policy"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        Task<TValue> ExecuteAsync<TValue>(Func<Task<TValue>> action, RetryPolicy policy = RetryPolicy.Infinite, TimeSpan? delay = null);
    }
}
