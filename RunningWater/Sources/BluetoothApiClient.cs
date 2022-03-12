using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RunningWater.Interfaces;

namespace RunningWater.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class GattCharacteristicAttribute : Attribute
    {
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public GattCharacteristicAttribute(string id)
            => Id = id;
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
            [GattCharacteristic("12345678-1234-5678-1234-56789abcdef1")]
            State,

            /// <summary>
            /// 
            /// </summary>
            [GattCharacteristic("12345678-1234-5678-1234-56789abcdef2")]
            Cron,
        }

        private readonly IBluetooth bluetooth;
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="write"></param>
        public BluetoothApiClient(IBluetooth bluetooth)
            => this.bluetooth = bluetooth;

        /// <inheritdoc/>
        public Task TryConnectAsync()
            => bluetooth.TryConnectAsync();

        /// <inheritdoc/>
        public Task StateWriteAsync(bool enabled)
            => WriteAsync(Property.State, new { enabled });

        /// <inheritdoc/>
        public Task<bool> StateReadAsync()
            => ReadAsync<bool>(Property.State);

        /// <inheritdoc/>
        public Task CronWriteAsync(string cron)
            => WriteAsync(Property.Cron, new { cron });

        /// <inheritdoc/>
        public Task<string> CronReadAsync()
            => ReadAsync<string>(Property.Cron);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private async Task WriteAsync(Property property, object values = null)
        {
            await bluetooth.TryConnectAsync();

            await bluetooth.WriteAsync(property.GetAttribute<GattCharacteristicAttribute>().Id, SerializePayload(values));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private async Task<TValue> ReadAsync<TValue>(Property property)
        {
            await bluetooth.TryConnectAsync();

            return DeserializePayload<TValue>(await bluetooth.ReadAsync(property.GetAttribute<GattCharacteristicAttribute>().Id));
        }

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
        private static TValue DeserializePayload<TValue>(byte[] bytes) => JsonSerializer.Deserialize<TValue>(Encoding.UTF8.GetString(bytes));
    }
}
