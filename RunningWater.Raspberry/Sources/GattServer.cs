using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DotnetBleServer.Advertisements;
using DotnetBleServer.Core;
using DotnetBleServer.Gatt;
using DotnetBleServer.Gatt.Description;
using RunningWater.Raspberry.Converters;
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
        public static GattServiceBuilder WithCharacteristic(this GattServiceBuilder builder, string id, Func<object> read, Action<IDictionary<string, object>> write)
        {
            builder.WithCharacteristic(new GattCharacteristicDescription
            {
                UUID = id,
                Flags = new string[] { "read", "write", "writable-auxiliaries" },
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
        public async Task ExecuteAsync()
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

            var context = new ServerContext();

            await context.Connect();

            const string serviceId = "12345678-1234-5678-1234-56789abcdef0";
            const string stateId = "12345678-1234-5678-1234-56789abcdef1";
            const string jobId = "12345678-1234-5678-1234-56789abcdef2";

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
                    () => logic.Execute(nameof(ILogicService.StateRead)),
                    value => logic.Execute(nameof(ILogicService.StateWrite), value))

                .WithCharacteristic(jobId,
                    () => logic.Execute(nameof(ILogicService.JobsRead)),
                    value => logic.Execute(nameof(ILogicService.JobsWrite), value));

            await new GattApplicationManager(context)
                .RegisterGattApplication(builder.BuildServiceDescriptions());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GenericCharacteristic : ICharacteristicSource
    {
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        private Func<object> read;
        private Action<IDictionary<string, object>> write;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="read"></param>
        /// <param name="write"></param>
        public GenericCharacteristic(Func<object> read, Action<IDictionary<string, object>> write)
        {
            this.read = read;
            this.write = write;

            jsonOptions.Converters.Add(new DateTimeUnixTimeConverter());
        }

        /// <inheritdoc/>
        public Task WriteValueAsync(byte[] value)
        {
            try
            {
                $"Write value -> {value.Length} bytes".Console();

                var request = JsonSerializer.Deserialize<IDictionary<string, object>>(Decompress(value), jsonOptions);

                write(request);

                "Write finished".Console();
            }
            catch (Exception exception)
            {
                exception.ToString().Console();
            }

            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public Task<byte[]> ReadValueAsync()
        {
            try
            {
                "Read value".Console();

                var value = Compress(JsonSerializer.SerializeToUtf8Bytes(read(), options: jsonOptions));

                $"Read finished -> {value.Length} bytes".Console();

                return Task.FromResult(value);
            }
            catch (Exception exception)
            {
                exception.ToString().Console();
                return Task.FromResult(new byte[] { });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static byte[] Compress(byte[] bytes)
        {
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                using (var compressor = new BrotliStream(output, CompressionLevel.Optimal))
                {
                    input.CopyTo(compressor);
                }

                return output.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static byte[] Decompress(byte[] bytes)
        {
            using (var input = new MemoryStream(bytes))
            using (var output = new MemoryStream())
            {
                using (var decompressor = new BrotliStream(input, CompressionMode.Decompress))
                {
                    decompressor.CopyTo(output);
                }

                return output.ToArray();
            }
        }
    }
}
