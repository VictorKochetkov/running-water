using System.Globalization;
using System.Reflection;
using System.Resources;

namespace RunninWater.Resources
{
    /// <summary>
    /// https://mindofai.github.io/Implementing-Localization-..
    /// </summary>
    public static class Translator
    {
        private readonly static ResourceManager resourceManager
            = new ResourceManager("RunningWater.Resources.AppResources", typeof(Translator).GetTypeInfo().Assembly);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(this string key)
            => resourceManager.GetString(key, CultureInfo.CurrentCulture) ?? key;
    }

}