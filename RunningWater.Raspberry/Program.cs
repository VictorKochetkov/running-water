using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RunningWater.Raspberry.Interfaces;
using RunningWater.Raspberry.Sources;
using RunningWater.Raspberry.Util;

namespace RunningWater.Raspberry
{
    /// <summary>
    /// Custom Hangfire job activator, because Hangfire can't do it by it self.
    /// </summary>
    public class DefaultJobActivator : JobActivator
    {
        private readonly IServiceProvider provider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public DefaultJobActivator(IServiceCollection services)
            => provider = services.BuildServiceProvider();

        /// <inheritdoc/>
        public override object ActivateJob(Type jobType)
            => provider.GetRequiredService(jobType);
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Start BLE GATT server
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddGattServer(this IServiceCollection services)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _ = services.BuildServiceProvider().GetRequiredService<IGattServer>().ExecuteAsync();
            }

            return services;
        }

        /// <summary>
        /// Disable USB controllers on service startup.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection DisableUsb(this IServiceCollection services)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                services.BuildServiceProvider().GetRequiredService<IUsbController>().DisableUsb();
            }

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection EnablePowerSave(this IServiceCollection services)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                "echo \"powersave\"| sudo tee /sys/devices/system/cpu/cpu0/cpufreq/scaling_governor".Bash();
            }

            return services;
        }

        /// <summary>
        /// Disable HDMI output on startup.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection DisableHdmi(this IServiceCollection services)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                "sudo tvservice -o".Bash();
            }

            return services;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class Startup : IDashboardAuthorizationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public void Configure(IApplicationBuilder app, IConfiguration config)
        {
            if (config.GetValue<bool>("Hangfire:Dashboard"))
            {
                app.UseHangfireDashboard(options: new DashboardOptions
                {
                    Authorization = new[] { this }
                });
            }
        }

        bool IDashboardAuthorizationFilter.Authorize(DashboardContext context) => true;
    }

    /// <summary>
    /// 
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            "Host started".Console();

            await CreateHostBuilder(args).Build().RunAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(config =>
                {
                    string fileName = args.ElementAtOrDefault(0) ?? "appsettings.json";

                    $"Using {fileName}".Console();

                    config.AddJsonFile(fileName, optional: false, reloadOnChange: false);
                })
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
                .ConfigureServices(services =>
                    services
                        .AddSingleton<IUsbController, UhubctlUsbController>()
                        .DisableUsb()
                        .DisableHdmi()
                        .EnablePowerSave()
                        .AddSingleton<IStorage, JsonFileStorage>()
                        .AddSingleton<IWateringJob, WateringJob>()
                        .AddSingleton<IGattServer, GattServer>()
                        .AddSingleton<ILogicService, LogicService>()
                        .AddSingleton<IJobScheduler<IWateringJob>, HangfireJobScheduler<IWateringJob>>()
                        .AddHangfire(config => config
                            .UseMemoryStorage()
                            .UseActivator(new DefaultJobActivator(services)))
                        .AddHangfireServer()
                        .AddGattServer()
                        );
        }
    }
}
