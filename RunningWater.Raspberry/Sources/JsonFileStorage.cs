using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using RunningWater.Raspberry.Interfaces;
using RunningWater.Raspberry.Util;

namespace RunningWater.Raspberry.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonFileStorage : IStorage
    {
        private static Dictionary<string, object> values = new Dictionary<string, object>();
        private static readonly SemaphoreSlim locker = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 
        /// </summary>
        public JsonFileStorage()
        {
            Locked(() =>
            {
                if (!File.Exists(ConfigPath))
                {
                    return;
                }

                using (var file = File.OpenRead(ConfigPath))
                {
                    values = JsonSerializer.Deserialize<Dictionary<string, object>>(file);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public TValue GetValue<TValue>(string key, TValue defaultValue = default(TValue)) => Locked(() =>
        {
            if (values.ContainsKey(key))
            {
                if (values[key] is JsonElement jsonElement && jsonElement.TryToObject(out TValue value))
                    values[key] = value;

                if (values[key] is TValue typedValue)
                    return typedValue;
            }

            return defaultValue;
        });

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, object value) => Locked(() =>
        {
            if (!values.TryAdd(key, value))
            {
                values[key] = value;
            }

            using (var file = File.OpenWrite(ConfigPath))
            {
                JsonSerializer.Serialize(file, values);
            }
        });

        /// <summary>
        /// 
        /// </summary>
        private static string ConfigPath => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? string.Empty, "config.json");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private static void Locked(Action action) => Locked(() =>
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
        private static TValue Locked<TValue>(Func<TValue> action)
        {
            locker.Wait();

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
