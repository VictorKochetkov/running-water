using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.LiteDB;
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
        /// Bluetooth GATT server
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
    }

    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public void Configure(IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
        }
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
                .ConfigureAppConfiguration(config => config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true))
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
                .ConfigureServices(services =>
                    services
                        .AddSingleton<IUsbController, UhubctlUsbController>()
                        .AddSingleton<IStorage, JsonFileStorage>()
                        .AddSingleton<IWateringJob, WateringJob>()
                        .AddSingleton<IGattServer, GattServer>()
                        .AddSingleton<ILogicService, LogicService>()
                        .AddSingleton<IJobScheduler, HangfireJobScheduler>()
                        .AddGattServer()
                        .AddHangfire(config => config
                            .UseLiteDbStorage()
                            .UseActivator(new DefaultJobActivator(services)))
                        .AddHangfireServer()
                        );
        }
    }
}
