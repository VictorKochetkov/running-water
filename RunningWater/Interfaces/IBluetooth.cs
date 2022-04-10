using System;
using System.Threading.Tasks;

namespace RunningWater.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBluetooth
    {
        /// <summary>
        /// Is target bluetooth device connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Try to connect to target device.
        /// If device wasn't connected earlier - searching will be performed.
        /// </summary>
        /// <returns></returns>
        Task TryConnectAsync();

        /// <summary>
        /// Write bytes array to GATT characterictic.
        /// </summary>
        /// <param name="characteristicId">Characteristic id.</param>
        /// <param name="data">Bytes array to be written.</param>
        /// <returns>Awaitable task.</returns>
        Task WriteAsync(Guid characteristicId, byte[] data);

        /// <summary>
        /// Read bytes array from GATT characterictic.
        /// </summary>
        /// <param name="characteristicId">Characteristic id.</param>
        /// <returns>Awaitable task.</returns>
        Task<byte[]> ReadAsync(Guid characteristicId);
    }
}
