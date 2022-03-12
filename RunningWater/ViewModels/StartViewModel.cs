using System.Threading.Tasks;
using RunningWater.Interfaces;
using RunningWater.Pages;

namespace RunningWater.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class StartViewModel : BaseViewModel
    {
        private readonly IApiClient apiClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="apiClient"></param>
        public StartViewModel(INavigationService navigationService, IApiClient apiClient)
            : base(navigationService)
        {
            this.apiClient = apiClient;
        }

        /// <inheritdoc/>
        public override async Task OnAppearing()
        {
            await base.OnAppearing();

            await DoTask(
                async () =>
                {
                    await apiClient.TryConnectAsync();
                    await Navigation.NavigateAsync<MainPage>();
                },
                minorLoading: true);

            //await apiClient.SetCronAsync("*/2 * * * *");
            //await apiClient.EnableAsync();
            //await apiClient.SetCronAsync("*/1 * * * *");
        }
    }
}