using System;
using System.Threading.Tasks;
using RunningWater.Interfaces;
using RunningWater.Pages;
using RunningWater.ViewModels;

namespace RunningWater.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class NavigationService : INavigationService
    {
        /// <inheritdoc/>
        public Task NavigateAsync<TPage>(params object[] arguments) where TPage : BasePage, new()
        {
            var page = new TPage();

            var viewModel = (BasePageViewModel)page.BindingContext;
            viewModel.Prepare(arguments);

            return App.Current.MainPage.Navigation.PushAsync(page);
        }
    }
}
