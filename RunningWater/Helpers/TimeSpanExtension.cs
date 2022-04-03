using System;
namespace RunningWater.Helpers
{
    public static class TimeSpanExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToLocalizedString(this TimeSpan value)
            => DateTime.Now.Date.Add(value).ToString("hh:mm tt");
    }
}
