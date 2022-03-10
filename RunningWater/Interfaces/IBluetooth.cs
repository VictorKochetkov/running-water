using System.Threading.Tasks;

namespace RunningWater.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBluetooth
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task TryConnectAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task WriteAsync(byte[] data);
    }
}
