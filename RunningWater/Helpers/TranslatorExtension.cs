using System;
using RunninWater.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RunningWater.Helpers
{
    /// <summary>
    /// https://mindofai.github.io/Implementing-Localization-..
    /// </summary>
    [ContentProperty("Key")]
    public class TranslateExtension : IMarkupExtension
    {
        public string Key { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
            => Translator.GetString(Key);
    }
}