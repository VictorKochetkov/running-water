using RunningWater.Raspberry.Interfaces;
using RunningWater.Raspberry.Util;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RunningWater.Raspberry.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class WateringJob : IWateringJob
    {
        private readonly IUsbController usbController;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="usbController"></param>
        public WateringJob(IUsbController usbController)
        {
            this.usbController = usbController;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task ExecuteAsync()
        {
            "Watering job started".Console();

            // Turn on power on all USB ports 
            usbController.EnableUsb();

            // Waiting for water pump to do his job
            await Task.Delay(TimeSpan.FromSeconds(3));

            // Then turn off power on all USB port again
            usbController.DisableUsb();

            "Watering job finished".Console();
        }
    }
}
