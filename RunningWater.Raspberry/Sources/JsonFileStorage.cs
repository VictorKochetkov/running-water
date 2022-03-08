using RunningWater.Raspberry.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RunningWater.Raspberry.Sources
{
    public class JsonFileStorage : IStorage
    {
        private static readonly SemaphoreSlim locker = new SemaphoreSlim(1, 1);
        private static readonly Dictionary<string, object> values = new Dictionary<string, object>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public Task<TValue> GetValueAsync<TValue>(string key, TValue defaultValue = default(TValue)) => LockedAsync(() =>
        {
            if (values.TryGetValue(key, out object value) && value is TValue typedValue)
            {
                return typedValue;
            }

            return defaultValue;
        });

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task SetValueAsync<TValue>(string key, TValue value) => LockedAsync(() =>
        {
            if (!values.TryAdd(key, value))
            {
                values[key] = value;
            }

            return WriteJsonAsync();
        });

        /// <summary>
        /// 
        /// </summary>
        private static async Task WriteJsonAsync()
        {
            string path = Assembly.GetEntryAssembly().Location;
            string directory = Path.GetDirectoryName(path);
            string jsonFilePath = Path.Combine(directory, "config.json");

            using (var file = File.OpenWrite(jsonFilePath))
            {
                await JsonSerializer.SerializeAsync(file, values);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private static Task LockedAsync(Action action) => LockedAsync(() =>
        {
            action();
            return true;
        });

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        private static async Task<TValue> LockedAsync<TValue>(Func<TValue> action)
        {
            await locker.WaitAsync();

            try
            {
                return action();
            }
            finally
            {
                locker.Release();
            }
        }
    }
}
