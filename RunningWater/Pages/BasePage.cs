using System;
using RunningWater.ViewModels;
using Xamarin.Forms;

namespace RunningWater.Pages
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BasePage : ContentPage
    {
        public BasePage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
        }

        /// <inheritdoc/>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as BasePageViewModel)?.OnAppearing();
        }

        /// <inheritdoc/>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (BindingContext as BasePageViewModel)?.OnDisappearing();
        }
    }
}
