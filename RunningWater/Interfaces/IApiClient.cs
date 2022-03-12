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
        /// <param name="cron"></param>
        /// <returns></returns>
        Task CronWriteAsync(string cron);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<string> CronReadAsync();
    }
}
