using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DotnetBleServer.Advertisements;
using DotnetBleServer.Core;
using DotnetBleServer.Gatt;
using DotnetBleServer.Gatt.Description;
using RunningWater.Raspberry.Attributes;
using RunningWater.Raspberry.Interfaces;
using RunningWater.Raspberry.Util;

namespace RunningWater.Raspberry.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public static class GattExtensions
    {
        /// <summary>
        /// "12345678-1234-5678-1234-56789abcdef1"
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static GattServiceBuilder WithCharacteristic(this GattServiceBuilder builder, string id, Func<Task<object>> read, Func<IDictionary<string, object>, Task> write)
        {
            builder.WithCharacteristic(new GattCharacteristicDescription
            {
                UUID = id,
                Flags = new[] { "read", "write", "writable-auxiliaries" },
                CharacteristicSource = new GenericCharacteristic(read, write),
            },
            new[]
            {
                new GattDescriptorDescription
                {
                    UUID = "12345678-1234-5678-1234-56789abcdef2",
                    Flags = new[] { "read", "write" },
                    Value = new[] { (byte)'t' },
                }
            });

            return builder;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GattServer : IGattServer
    {
        private readonly ILogicService logic;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logic"></param>
        public GattServer(ILogicService logic)
            => this.logic = logic;

        /// <summary>
        /// 
        /// </summary>
        public Task ExecuteAsync()
        {
            // Disable bluetooth discovery timeout
            "sudo sed -i 's/^#DiscoverableTimeout = .*/DiscoverableTimeout = 0/' /etc/bluetooth/main.conf".Bash();

            // Turn on bluetooth
            "sudo bluetoothctl power on".Bash();

            // Make bluetooth discoverable
            "sudo bluetoothctl discoverable on".Bash();

            // Disable pairing to prevent system popup displaying on mobile app
            "sudo bluetoothctl pairable off".Bash();

            // Disable pairing to prevent system popup displaying on mobile app
            "sudo bluetoothctl agent NoInputNoOutput".Bash();


            // BLE GATT server configuration
            Task.Run(async () =>
            {
                using (var context = new ServerContext())
                {
                    await context.Connect();

                    const string serviceId = "12345678-1234-5678-1234-56789abcdef0";
                    const string stateId = "12345678-1234-5678-1234-56789abcdef1";
                    const string cronId = "12345678-1234-5678-1234-56789abcdef2";

                    await new AdvertisingManager(context).CreateAdvertisement(new AdvertisementProperties
                    {
                        Type = "peripheral",
                        ServiceUUIDs = new[] { serviceId },
                        LocalName = "Running water",
                    });

                    var builder = new GattApplicationBuilder();

                    builder
                        .AddService(new GattServiceDescription
                        {
                            UUID = serviceId,
                            Primary = true
                        })

                        .WithCharacteristic(stateId,
                            () => logic.ExecuteAsync(nameof(ILogicService.StateRead)),
                            value => logic.ExecuteAsync(nameof(ILogicService.StateWrite), value))

                        .WithCharacteristic(cronId,
                            () => logic.ExecuteAsync(nameof(ILogicService.CronRead)),
                            value => logic.ExecuteAsync(nameof(ILogicService.CronWrite), value));

                    await new GattApplicationManager(context)
                        .RegisterGattApplication(builder.BuildServiceDescriptions());

                    await TaskHelper.WaitInfinite();
                }
            });

            return Task.FromResult(true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GenericCharacteristic : ICharacteristicSource
    {
        private Func<Task<object>> read;
        private Func<IDictionary<string, object>, Task> write;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="read"></param>
        /// <param name="write"></param>
        public GenericCharacteristic(Func<Task<object>> read, Func<IDictionary<string, object>, Task> write)
        {
            this.read = read;
            this.write = write;
        }

        /// <inheritdoc/>
        public async Task WriteValueAsync(byte[] value)
        {
            try
            {
                "Write value".Console();

                var request = JsonSerializer.Deserialize<IDictionary<string, object>>(value);

                await write(request);

                "Write finished".Console();
            }
            catch (Exception exception)
            {
                exception.ToString().Console();
            }
        }

        /// <inheritdoc/>
        public async Task<byte[]> ReadValueAsync()
        {
            try
            {
                "Read value".Console();

                var result = await read();

                "Read finished".Console();

                return JsonSerializer.SerializeToUtf8Bytes(result);
            }
            catch (Exception exception)
            {
                exception.ToString().Console();
                return new byte[] { };
            }
        }
    }
}
