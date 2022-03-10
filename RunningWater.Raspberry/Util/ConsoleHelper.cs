using System;

namespace RunningWater.Raspberry.Util
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConsoleHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Console(this string message)
            => System.Console.WriteLine($"[{DateTime.Now}] {message}");
    }
}
