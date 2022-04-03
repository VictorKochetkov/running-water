using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using RunningWater.Raspberry.Converters;

namespace RunningWater.Raspberry.Util
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="methodName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static object Execute(this object source, string methodName, IDictionary<string, object> values = null)
        {
            try
            {
                var targetMethod = source.GetType().GetMethods().SingleOrDefault(method => method.Name == methodName);

                if (targetMethod == null)
                    throw new NullReferenceException($"Method `{methodName}` not found");

                var args = targetMethod
                    .GetParameters()
                    .Select(parameter =>
                    {
                        if (values is not null && values.TryGetValue(parameter.Name, out object value) && value is JsonElement jsonElement)
                        {
                            var parameterType = Nullable.GetUnderlyingType(parameter.ParameterType) ?? parameter.ParameterType;
                            return Convert(jsonElement, parameterType);
                        }

                        return null;
                    })
                    .ToArray();


                // Запускаем метод
                var result = targetMethod.Invoke(source, args);

                // Это task? -> ждем завершения
                if (result is Task task)
                {
                    throw new Exception();
                }

                return result;
            }
            catch (Exception exception)
            {
                $"Exception occurred while executing `{methodName}` -> {exception}".Console();
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static object Convert(JsonElement? value, Type targetType)
        {
            if (targetType == typeof(string))
                return value?.GetString();

            if (targetType == typeof(int))
                return value?.GetInt32();

            if (targetType == typeof(bool))
                return value?.GetBoolean();

            var bufferWriter = new ArrayBufferWriter<byte>();

            using (var writer = new Utf8JsonWriter(bufferWriter))
                value?.WriteTo(writer);

            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new DateTimeUnixTimeConverter());

            return JsonSerializer.Deserialize(bufferWriter.WrittenSpan, targetType, jsonOptions);
        }
    }
}
