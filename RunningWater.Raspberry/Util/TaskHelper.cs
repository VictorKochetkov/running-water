using System.Threading.Tasks;

namespace RunningWater.Raspberry.Util
{
    /// <summary>
    /// 
    /// </summary>
    public static class TaskHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Task WaitInfinite() => Task.Delay(-1);
    }
}
