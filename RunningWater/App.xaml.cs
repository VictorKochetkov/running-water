using System;
using Microsoft.Extensions.DependencyInjection;
using RunningWater.Interfaces;
using RunningWater.Pages;
using RunningWater.Sources;
using RunningWater.ViewModels;
using Xamarin.Forms;

namespace RunningWater
{
    /// <summary>
    /// 
    /// </summary>
    public partial class App : Application
    {
        private static IServiceProvider provider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addPlatformServices"></param>
        public App(Action<IServiceCollection> addPlatformServices = null)
        {
            InitializeComponent();

            var services = new ServiceCollection();

            // Add platform specific services
            addPlatformServices?.Invoke(services);

            // Add core services
            services.AddSingleton<IApiClient, BluetoothApiClient>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IRetryHandlerService, RetryHandlerService>();

            // Add view models
            services.AddTransient<StartViewModel>();
            services.AddTransient<MainViewModel>();

            provider = services.BuildServiceProvider();

            MainPage = new NavigationPage(new StartPage());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <returns></returns>
        public static BasePageViewModel GetViewModel<TViewModel>() where TViewModel : BasePageViewModel
            => provider.GetRequiredService<TViewModel>();

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
