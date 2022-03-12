using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using RunningWater.Interfaces;
using Xamarin.Forms;

namespace RunningWater.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly IApiClient apiClient;

        /// <summary>
        /// 
        /// </summary>
        [Reactive]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Reactive]
        public string Cron { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand CronCommand => new Command(async () =>
        {
            await DoTask(async () =>
            {
                await apiClient.CronWriteAsync(Cron);
                Cron = await apiClient.CronReadAsync();
            });
        });

        /// <summary>
        /// 
        /// </summary>
        public ICommand StateCommand => new Command(async () =>
        {
            await DoTask(async () =>
            {
                await apiClient.StateWriteAsync(IsEnabled);
                IsEnabled = await apiClient.StateReadAsync();
            });
        });

        public MainViewModel(INavigationService navigationService, IApiClient apiClient)
            : base(navigationService)
        {
            this.apiClient = apiClient;

            this
                .WhenAnyValue(viewModel => viewModel.Cron)
                .Skip(1)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .InvokeCommand(CronCommand);

            this
                .WhenAnyValue(viewModel => viewModel.IsEnabled)
                .Skip(1)
                .InvokeCommand(StateCommand);
        }

        /// <inheritdoc/>
        public override async Task OnAppearing()
        {
            await base.OnAppearing();

            await DoTask(async () =>
            {
                Cron = await apiClient.CronReadAsync();
                IsEnabled = await apiClient.StateReadAsync();
            });
        }

    }
}
