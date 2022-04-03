using System;
using RunningWater.Helpers;

namespace RunningWater.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class TimeItemViewModel : BaseViewModel
    {
        public TimeItemViewModel(TimeSpan source)
            => Time = source;

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Time { get; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Value => Time.ToString(@"hh\:mm");
    }
}
