using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RunningWater.Raspberry.Interfaces;
using RunningWater.Raspberry.Sources;
using RunningWater.Raspberry.Util;
using System;
using System.Threading.Tasks;

namespace RunningWater.Raspberry
{
    internal class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            Console.WriteLine("Hello Bluetooth");

            // Disable bluetooth discovery timeout
            "sudo sed -i 's/^#DiscoverableTimeout = .*/DiscoverableTimeout = 0/' /etc/bluetooth/main.conf".Bash();

            "sudo bluetoothctl power on".Bash();
            "sudo bluetoothctl discoverable on".Bash();
            "sudo bluetoothctl pairable off".Bash();

            // Periodic watering job
            host.Services.GetRequiredService<IWateringJob>().Start();

            // Bluetooth Gatt server
            host.Services.GetRequiredService<IGattServer>().Start();

            Console.WriteLine("Press CTRL+C to quit");

            return host.RunAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                    services
                        .AddSingleton<IUsbController, UhubctlUsbController>()
                        .AddSingleton<IStorage, JsonFileStorage>()
                        .AddSingleton<IWateringJob, WateringJob>()
                        .AddSingleton<IGattServer, GattServer>()
                        );
        }
    }
}
