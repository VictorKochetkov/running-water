using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RunningWater.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task TryConnectAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        Task StateWriteAsync(bool enabled);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<bool> StateReadAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobs"></param>
        /// <returns></returns>
        Task JobsWriteAsync(IEnumerable<DateTimeOffset> jobs);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DateTimeOffset>> JobsReadAsync();
    }
}
