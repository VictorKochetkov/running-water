using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RunningWater.Interfaces;
using Shiny;
using Shiny.BluetoothLE;
using Xamarin.Essentials;

namespace RunningWater.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Bluetooth : IBluetooth
    {
        private const string SERVICE_ID = "12345678-1234-5678-1234-56789abcdef0";
        private const string DEVICE_ID = "Running water";

        private readonly static SemaphoreSlim locker = new SemaphoreSlim(1, 1);

        private IBleManager adapter;
        private IPeripheral device;
        private IList<IGattCharacteristic> characteristics;

        /// <summary>
        /// 
        /// </summary>
        public Bluetooth(IBleManager adapter)
            => this.adapter = adapter;

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnected => device?.IsConnected() == true;

        /// <inheritdoc/>
        public Task WriteAsync(string characteristicId, byte[] data)
        {
            return MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await locker.WaitAsync();

                try
                {
                    if (!IsConnected)
                        throw new InvalidOperationException("Device disconnected");

                    if (characteristics?.SingleOrDefault(x => x.Uuid == characteristicId) is not { } target)
                        throw new InvalidOperationException("Characteristic not found");

                    //var tcs = new TaskCompletionSource<bool>();

                    Console.WriteLine("Writing value");

                    await target.Write(data);

                    //await tcs.Task;

                    Console.WriteLine("Writing finished");
                }
                finally
                {
                    locker.Release();
                }
            });
        }

        /// <inheritdoc/>
        public Task<byte[]> ReadAsync(string characteristicId)
        {
            return MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await locker.WaitAsync();

                try
                {
                    if (!IsConnected)
                        throw new InvalidOperationException("Device disconnected");

                    if (characteristics?.SingleOrDefault(x => x.Uuid == characteristicId) is not { } target)
                        throw new InvalidOperationException("Characteristic not found");

                    //var tcs = new TaskCompletionSource<byte[]>();

                    Console.WriteLine("Reading value");

                    var result = await target.Read();

                    //var result = await tcs.Task;

                    Console.WriteLine("Reading finished");

                    return result.Data;
                }
                finally
                {
                    locker.Release();
                }
            });
        }

        /// <inheritdoc/>
        public async Task TryConnectAsync()
        {
            if (IsConnected)
                return;

            if (await adapter.RequestAccess() != AccessState.Available)
                throw new SecurityException("Permissions not granted");

            if (device == null)
            {
                Console.WriteLine("Searching for device");
                device = await adapter.ScanUntilPeripheralFound(DEVICE_ID);
            }

            Console.WriteLine("Connecting to device");

            await device.ConnectAsync(new ConnectionConfig
            {
                AndroidConnectionPriority = ConnectionPriority.High,
                AutoConnect = true,
            });

            Console.WriteLine("Reading service and characteristics");

            characteristics = await (await device
                .GetKnownService(SERVICE_ID, throwIfNotFound: true))
                .GetCharacteristicsAsync();

            if (!IsConnected)
                throw new Exception("Connection failed");

            Console.WriteLine("Connection successfull");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract Task EnableBluetoothAsync();
    }
}
