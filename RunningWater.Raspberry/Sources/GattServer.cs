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
    public class ActionRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("a")]
        public string Action { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("v")]
        public IDictionary<string, object> Values { get; set; }
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

                    await new AdvertisingManager(context).CreateAdvertisement(new AdvertisementProperties
                    {
                        Type = "peripheral",
                        ServiceUUIDs = new[] { "12345678-1234-5678-1234-56789abcdef0" },
                        LocalName = "Raspberry BLE",
                    });

                    var builder = new GattApplicationBuilder();

                    builder
                        .AddService(new GattServiceDescription
                        {
                            UUID = "12345678-1234-5678-1234-56789abcdef0",
                            Primary = true
                        })
                        .WithCharacteristic(new GattCharacteristicDescription
                        {
                            UUID = "12345678-1234-5678-1234-56789abcdef1",
                            Flags = new[] { "read", "write", "writable-auxiliaries" },
                            CharacteristicSource = new GenericCharacteristic(
                                async () =>
                                {
                                    "Read value".Console();

                                    try
                                    {
                                        return await Task.FromResult(new byte[] { 123 });
                                    }
                                    catch (Exception exception)
                                    {
                                        exception.ToString().Console();
                                        throw;
                                    }
                                },
                                async value =>
                                {
                                    "Write value".Console();

                                    try
                                    {
                                        var request = JsonSerializer.Deserialize<ActionRequest>(value);

                                        $"Action -> {request.Action}".Console();

                                        await logic.ExecuteAsync(request.Action, request.Values);
                                    }
                                    catch (Exception exception)
                                    {
                                        exception.ToString().Console();
                                        throw;
                                    }
                                }),
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
        private Func<Task<byte[]>> read;
        private Func<byte[], Task> write;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="read"></param>
        /// <param name="write"></param>
        public GenericCharacteristic(Func<Task<byte[]>> read, Func<byte[], Task> write)
        {
            this.read = read;
            this.write = write;
        }

        /// <inheritdoc/>
        public Task WriteValueAsync(byte[] value) => write(value);

        /// <inheritdoc/>
        public Task<byte[]> ReadValueAsync() => read();
    }
}
