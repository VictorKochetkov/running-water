using System;
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
    public class BluetoothApiClient : IApiClient
    {
        /// <summary>
        /// 
        /// </summary>
        private class ActionRequest
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
            public object Values { get; set; }
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
        public Task EnableAsync()
            => RequestAsync("job/state/enable");

        /// <inheritdoc/>
        public Task DisableAsync()
            => RequestAsync("job/state/disable");

        /// <inheritdoc/>
        public Task SetCronAsync(string cron)
            => RequestAsync("job/cron/set", new { cron });

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private async Task RequestAsync(string action, object values = null)
        {
            await bluetooth.TryConnectAsync();

            await bluetooth.WriteAsync(BuildRequest(new ActionRequest
            {
                Action = action,
                Values = values,
            }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static byte[] BuildRequest(ActionRequest request) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request, jsonOptions));
    }
}
