using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RunningWater.Converters;
using RunningWater.Interfaces;

namespace RunningWater.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class GattCharacteristicAttribute : Attribute
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public GattCharacteristicAttribute(string id)
            => Id = Guid.Parse(id);
    }

    /// <summary>
    /// 
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
            => value
                .GetType()
                .GetMember(value.ToString())
                .First()
                .GetCustomAttribute<TAttribute>();
    }

    /// <summary>
    /// 
    /// </summary>
    public class BluetoothApiClient : IApiClient
    {
        /// <summary>
        /// 
        /// </summary>
        private enum Property
        {
            /// <summary>
            /// 
            /// </summary>
            [GattCharacteristic("13333333333333333333333333330002")]
            Jobs,
        }

        private readonly IBluetooth bluetooth;
        private readonly IRetryHandlerService retryHandler;

        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bluetooth"></param>
        /// <param name="retryHandler"></param>
        public BluetoothApiClient(IBluetooth bluetooth, IRetryHandlerService retryHandler)
        {
            this.bluetooth = bluetooth;
            this.retryHandler = retryHandler;

            jsonOptions.Converters.Add(new DateTimeUnixTimeConverter());
        }

        /// <inheritdoc/>
        public Task TryConnectAsync()
            => retryHandler.ExecuteAsync(() => bluetooth.TryConnectAsync());

        /// <inheritdoc/>
        public Task JobsWriteAsync(IEnumerable<DateTimeOffset> jobs)
            => WriteAsync(Property.Jobs, new { jobs });

        /// <inheritdoc/>
        public Task<IEnumerable<DateTimeOffset>> JobsReadAsync()
            => ReadAsync<IEnumerable<DateTimeOffset>>(Property.Jobs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private Task WriteAsync(Property property, object values = null)
            => retryHandler.ExecuteAsync(async () =>
            {
                await bluetooth.TryConnectAsync();
                await bluetooth.WriteAsync(property.GetAttribute<GattCharacteristicAttribute>().Id, SerializePayload(values));
            });

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private Task<TValue> ReadAsync<TValue>(Property property)
            => retryHandler.ExecuteAsync(async () =>
            {
                await bluetooth.TryConnectAsync();
                return DeserializePayload<TValue>(await bluetooth.ReadAsync(property.GetAttribute<GattCharacteristicAttribute>().Id));
            });

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private static byte[] SerializePayload(object payload) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload, jsonOptions));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static TValue DeserializePayload<TValue>(byte[] bytes) => JsonSerializer.Deserialize<TValue>(Encoding.UTF8.GetString(bytes), jsonOptions);
    }
}
