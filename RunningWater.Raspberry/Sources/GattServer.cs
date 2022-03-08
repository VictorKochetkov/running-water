using DotnetBleServer.Advertisements;
using DotnetBleServer.Core;
using DotnetBleServer.Gatt;
using DotnetBleServer.Gatt.Description;
using RunningWater.Raspberry.Interfaces;
using RunningWater.Raspberry.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningWater.Raspberry.Sources
{
    /// <summary>
    /// 
    /// </summary>
    internal class MobileAppBackendCharacteristic : ICharacteristicSource
    {
        private readonly IWateringJob job;

        static int count = 0;

        public MobileAppBackendCharacteristic(IWateringJob job)
        {
            this.job = job;
        }

        public Task WriteValueAsync(byte[] value)
        {
            Console.WriteLine("Writing value");
            return Task.Run(() => Console.WriteLine(Encoding.ASCII.GetChars(value)));
        }

        public Task<byte[]> ReadValueAsync()
        {
            Console.WriteLine("Reading value");
            return Task.FromResult(Encoding.ASCII.GetBytes($"Hello BLE {count++}"));
        }
    }

    public class GattServer : IGattServer
    {
        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            // BLE GATT server configuration
            Task.Run(async () =>
            {
                using (var context = new ServerContext())
                {
                    await context.Connect();

                    await new AdvertisingManager(context).CreateAdvertisement(new AdvertisementProperties
                    {
                        Type = "peripheral",
                        ServiceUUIDs = new[] { "12345678-1234-5678-1234-56789abcdef0" },
                        LocalName = "Raspberry BLE",
                    });

                    var serviceDescription = new GattServiceDescription
                    {
                        UUID = "12345678-1234-5678-1234-56789abcdef0",
                        Primary = true
                    };

                    var characteristicDescription = new GattCharacteristicDescription
                    {
                        CharacteristicSource = new MobileAppBackendCharacteristic(null),
                        UUID = "12345678-1234-5678-1234-56789abcdef1",
                        Flags = new[] { "read", "write", "writable-auxiliaries" }
                    };

                    var descriptorDescription = new GattDescriptorDescription
                    {
                        Value = new[] { (byte)'t' },
                        UUID = "12345678-1234-5678-1234-56789abcdef2",
                        Flags = new[] { "read", "write" }
                    };

                    var gab = new GattApplicationBuilder();
                    gab
                        .AddService(serviceDescription)
                        .WithCharacteristic(characteristicDescription, new[] { descriptorDescription });

                    await new GattApplicationManager(context).RegisterGattApplication(gab.BuildServiceDescriptions());

                    await TaskHelper.WaitInfinite();
                }
            });

        }
    }
}
