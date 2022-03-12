using System;
using System.Threading.Tasks;
using RunningWater.Interfaces;
using RunningWater.Pages;

namespace RunningWater.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class NavigationService : INavigationService
    {
        /// <inheritdoc/>
        public Task NavigateAsync<TPage>() where TPage : BasePage, new()
            => App.Current.MainPage.Navigation.PushAsync(new TPage());
    }
}
