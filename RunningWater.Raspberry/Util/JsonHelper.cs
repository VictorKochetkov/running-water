using System;
using System.Buffers;
using System.Text.Json;

namespace RunningWater.Raspberry.Util
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class JsonHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="element"></param>
        /// <param name="typedValue"></param>
        /// <returns></returns>
        public static bool TryToObject<TValue>(this JsonElement element, out TValue typedValue)
        {
            try
            {
                var bufferWriter = new ArrayBufferWriter<byte>();

                using (var writer = new Utf8JsonWriter(bufferWriter))
                    element.WriteTo(writer);

                typedValue = JsonSerializer.Deserialize<TValue>(bufferWriter.WrittenSpan);
                return true;
            }
            catch
            {
                typedValue = default(TValue);
                return false;
            }
        }
    }
}
