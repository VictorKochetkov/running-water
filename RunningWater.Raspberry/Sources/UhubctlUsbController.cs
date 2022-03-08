using RunningWater.Raspberry.Interfaces;
using RunningWater.Raspberry.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace RunningWater.Raspberry.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class UhubctlUsbController : IUsbController
    {
        /// <summary>
        /// 
        /// </summary>
        public void EnableUsb()
            => "sudo /home/pi/uhubctl/uhubctl -a on -l 1-1".Bash();

        /// <summary>
        /// 
        /// </summary>
        public void DisableUsb()
            => "sudo /home/pi/uhubctl/uhubctl -a off -l 1-1".Bash();
    }
}
