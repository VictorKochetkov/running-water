using System.Threading.Tasks;
using RunningWater.Interfaces;
using RunningWater.Pages;

namespace RunningWater.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class StartViewModel : BasePageViewModel
    {
        private readonly IApiClient _apiClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        /// <param name="apiClient"></param>
        public StartViewModel(INavigationService navigationService, IDialogService dialogService, IApiClient apiClient)
            : base(navigationService, dialogService)
        {
            _apiClient = apiClient;
        }

        /// <inheritdoc/>
        public override async Task OnAppearing()
        {
            await base.OnAppearing();

            await DoTask(async () =>
            {
                await Navigation.NavigateAsync<MainPage>(
                    await _apiClient.StateReadAsync(),
                    await _apiClient.JobsReadAsync());
            });
        }
    }
}