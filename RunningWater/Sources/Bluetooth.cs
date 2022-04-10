using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using RunningWater.Interfaces;

namespace RunningWater.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class Bluetooth : IBluetooth
    {
        private const string SERVICE_ID = "13333333333333333333333333333337";
        private const string DEVICE_ID = "Running Water";

        private readonly static SemaphoreSlim serial = new SemaphoreSlim(1, 1);

        private IAdapter adapter;
        private IDevice device;
        private IReadOnlyList<ICharacteristic> characteristics;

        /// <summary>
        /// 
        /// </summary>
        public Bluetooth()
        {
            adapter = CrossBluetoothLE.Current.Adapter;
            adapter.ScanMode = ScanMode.LowLatency;
        }

        /// <inheritdoc/>
        public bool IsConnected => device?.State == DeviceState.Connected;

        /// <inheritdoc/>
        public async Task WriteAsync(Guid characteristicId, byte[] data)
        {
            await serial.WaitAsync();

            try
            {
                if (!IsConnected)
                    throw new InvalidOperationException("Device disconnected");

                if (characteristics?.SingleOrDefault(x => x.Uuid == characteristicId.ToString()) is not { } target)
                    throw new InvalidOperationException("Characteristic not found");

                if (!await target.WriteAsync(await CompressAsync(data)))
                    throw new Exception();
            }
            finally
            {
                serial.Release();
            }
        }

        /// <inheritdoc/>
        public async Task<byte[]> ReadAsync(Guid characteristicId)
        {
            await serial.WaitAsync();

            try
            {
                if (!IsConnected)
                    throw new InvalidOperationException("Device disconnected");

                if (characteristics?.SingleOrDefault(x => x.Uuid == characteristicId.ToString()) is not { } target)
                    throw new InvalidOperationException("Characteristic not found");

                return await DecompressAsync(await target.ReadAsync());
            }
            finally
            {
                serial.Release();
            }
        }

        /// <inheritdoc/>
        public async Task TryConnectAsync()
        {
            var a = Guid.Parse(SERVICE_ID);

            // Searching for device if needed
            device ??= await SearchDeviceAsync();

            // Connecting to found device
            await adapter.ConnectToDeviceAsync(device, new ConnectParameters(true, true));

            // Retrieving target GATT service and reading all it's characteristics
            characteristics = (await (await device
                .GetServicesAsync()).Single()
                .GetCharacteristicsAsync());

            if (!IsConnected)
                throw new Exception("Connection failed");
        }

        /// <summary>
        /// Search for target device.
        /// </summary>
        /// <returns>Found device.</returns>
        private async Task<IDevice> SearchDeviceAsync()
        {
            var tcs = new TaskCompletionSource<IDevice>();

            var scanTimeoutElapsed = new EventHandler(async (sender, args) =>
            {
                // Restarting BLE scanning when timeout occured
                // So device searching progress is infinite
                await adapter.StartScanningForDevicesAsync();
            });

            var deviceDiscovered = new EventHandler<DeviceEventArgs>((sender, args) =>
            {
                if (args.Device.Name == DEVICE_ID)
                {
                    tcs.TrySetResult(args.Device);
                }
            });

            adapter.ScanTimeoutElapsed += scanTimeoutElapsed;
            adapter.DeviceDiscovered += deviceDiscovered;

            // Start BLE scanning
            await adapter.StartScanningForDevicesAsync();

            // Waiting for search task to be completed
            var device = await tcs.Task;

            await adapter.StopScanningForDevicesAsync();

            adapter.ScanTimeoutElapsed -= scanTimeoutElapsed;
            adapter.DeviceDiscovered -= deviceDiscovered;

            return device;
        }

        /// <summary>
        /// Platform specific method to enabled bluetooth.
        /// </summary>
        /// <returns></returns>
        protected virtual Task EnableBluetoothAsync() => Task.FromResult(true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static async Task<byte[]> CompressAsync(byte[] bytes)
        {
            return bytes;
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                using (var compressor = new BrotliStream(output, CompressionLevel.Optimal))
                {
                    await input.CopyToAsync(compressor);
                }

                return output.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static async Task<byte[]> DecompressAsync(byte[] bytes)
        {
            return bytes;
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                using (var decompressor = new BrotliStream(input, CompressionMode.Decompress))
                {
                    await decompressor.CopyToAsync(output);
                }

                return output.ToArray();
            }
        }
    }
}
