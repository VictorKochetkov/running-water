using RunningWater.Raspberry.Interfaces;
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
        private readonly IStorage storage;
        private readonly IUsbController usbController;

        private CancellationTokenSource source;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="usbController"></param>
        public WateringJob(IStorage storage, IUsbController usbController)
        {
            this.storage = storage;
            this.usbController = usbController;
        }

        /// <summary>
        /// 
        /// </summary>
        private bool IsRunning
        {
            get
            {
                lock (typeof(WateringJob))
                {
                    return source != null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            lock (typeof(WateringJob))
            {
                if (IsRunning)
                {
                    Console.WriteLine("Job is already running");
                    return;
                }

                source = new CancellationTokenSource();

                usbController.DisableUsb();

                Task.Run(async () =>
                {
                    if (await GetPeriod() is not TimeSpan delay)
                        return;

                    while (IsRunning)
                    {
                        await Task.Delay(delay, source.Token);

                        if (!IsRunning)
                            return;

                        await Execute();
                    }
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private async Task SetPeriod(TimeSpan? value)
        {
            await storage.SetValueAsync("period", value);

            lock (typeof(WateringJob))
            {
                source?.Cancel();
                source = null;

                Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Task<TimeSpan?> GetPeriod() => storage.GetValueAsync<TimeSpan?>("period");

        /// <summary>
        /// 
        /// </summary>
        private async Task Execute()
        {
            // Turn on power on all USB ports 
            usbController.EnableUsb();

            // Waiting for water pump to do his job
            await Task.Delay(TimeSpan.FromSeconds(3));

            // Then turn off power on all USB port again
            usbController.DisableUsb();
        }
    }
}
