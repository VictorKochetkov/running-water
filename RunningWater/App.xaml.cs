using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RunningWater.Interfaces;
using RunningWater.Pages;
using RunningWater.Sources;
using RunningWater.ViewModels;
using Shiny;
using Shiny.BluetoothLE;
using Xamarin.Forms;

namespace RunningWater
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup : ShinyStartup
    {
        private readonly Action<IServiceCollection> addPlatformServices;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addPlatformServices"></param>
        public Startup(Action<IServiceCollection> addPlatformServices = null)
            => this.addPlatformServices = addPlatformServices;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="platform"></param>
        public override void ConfigureServices(IServiceCollection services, IPlatform platform)
        {
            // Add platform specific services
            addPlatformServices?.Invoke(services);

            // Bluetooth
            services.UseBleClient(new BleConfiguration
            {
                iOSShowPowerAlert = true,
                AndroidShouldInvokeOnMainThread = true,
            });

            // Add core services
            services.AddSingleton<IApiClient, BluetoothApiClient>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Add view models
            services.AddTransient<StartViewModel>();
            services.AddTransient<MainViewModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <returns></returns>
        public static BaseViewModel GetViewModel<TViewModel>() where TViewModel : BaseViewModel
            => ShinyHost.Resolve<TViewModel>();
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 
        /// </summary>
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new StartPage());
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnStart()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnSleep()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnResume()
        {
        }
    }
}
