using System.Threading.Tasks;
using RunningWater.Interfaces;

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
        /// <param name="apiClient"></param>
        public MainViewModel(IApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        /// <inheritdoc/>
        public override async Task OnAppearing()
        {
            await base.OnAppearing();

            await DoTask(() => apiClient.TryConnectAsync(), minorLoading: true);

            //await apiClient.SetCronAsync("*/2 * * * *");
            //await apiClient.EnableAsync();
            //await apiClient.SetCronAsync("*/1 * * * *");
        }
    }
}