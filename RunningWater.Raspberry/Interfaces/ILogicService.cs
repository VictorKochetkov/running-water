using System;
using System.Collections.Generic;

namespace RunningWater.Raspberry.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILogicService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<DateTimeOffset> JobsRead();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobs"></param>
        void JobsWrite(IEnumerable<DateTimeOffset> jobs);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool StateRead();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        void StateWrite(bool enabled);
    }
}
