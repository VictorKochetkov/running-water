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
        /// <returns></returns>
        Task EnableAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task DisableAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cron"></param>
        /// <returns></returns>
        Task SetCronAsync(string cron);
    }
}
