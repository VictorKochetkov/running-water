using System;
using RunningWater.ViewModels;
using Xamarin.Forms;

namespace RunningWater.Pages
{
    public abstract class BasePage : ContentPage
    {
        public BasePage()
        {
        }

        /// <inheritdoc/>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as BaseViewModel)?.OnAppearing();
        }

        /// <inheritdoc/>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (BindingContext as BaseViewModel)?.OnDisappearing();
        }
    }
}
