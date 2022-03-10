using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Security;
using System.Threading.Tasks;
using RunningWater.Interfaces;
using Shiny;
using Shiny.BluetoothLE;

namespace RunningWater.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Bluetooth : IBluetooth
    {
        private const string SERVICE_ID = "12345678-1234-5678-1234-56789abcdef0";
        private const string CHARACTERISTIC_ID = "12345678-1234-5678-1234-56789abcdef1";
        private const string DEVICE_ID = "Raspberry BLE";

        private IBleManager adapter;
        private IPeripheral device;
        private IGattCharacteristic characteristic;

        /// <summary>
        /// 
        /// </summary>
        public Bluetooth(IBleManager adapter)
        {
            this.adapter = adapter;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnected => device?.IsConnected() == true && characteristic?.CanWrite() == true;

        /// <inheritdoc/>
        public async Task WriteAsync(byte[] data)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Unable to perform write - device disconnected or characteristic not found or doesn't support write");

            await characteristic.Write(data);
        }

        /// <inheritdoc/>
        public async Task TryConnectAsync()
        {
            if (IsConnected)
                return;

            if (await adapter.RequestAccess() != AccessState.Available)
                throw new SecurityException("Permissions not granted");

            if (device == null)
                device = await adapter.ScanUntilPeripheralFound(DEVICE_ID);

            await device.ConnectAsync(new ConnectionConfig
            {
                AndroidConnectionPriority = ConnectionPriority.High,
                AutoConnect = true,
            });

            characteristic = await (await device
                .GetKnownService(SERVICE_ID, throwIfNotFound: true))
                .GetKnownCharacteristic(CHARACTERISTIC_ID, throwIfNotFound: true);

            if (!IsConnected)
                throw new Exception("Connection failed");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract Task EnableBluetoothAsync();
    }
}
