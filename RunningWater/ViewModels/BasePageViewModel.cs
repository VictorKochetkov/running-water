using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using RunningWater.Interfaces;
using Xamarin.CommunityToolkit.ObjectModel;

namespace RunningWater.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class BasePageViewModel : BaseViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly INavigationService Navigation;

        /// <summary>
        /// 
        /// </summary>
        protected readonly IDialogService Dialog;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        public BasePageViewModel(INavigationService navigationService, IDialogService dialogService)
            : base()
        {
            Navigation = navigationService;
            Dialog = dialogService;

            this.WhenAnyValue(viewModel => viewModel.State)
                .Subscribe(value => OnPropertyChanged(nameof(IsBusy)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Task OnAppearing()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Task OnDisappearing()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arguments"></param>
        public virtual void Prepare(params object[] arguments)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        protected ICommand BuildCommand<TValue>(Func<TValue, Task> task)
            => new AsyncCommand<TValue>(value => DoTask(() => task(value)), allowsMultipleExecutions: false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        protected ICommand BuildTaskCommand(Func<Task> task)
            => new AsyncCommand(() => DoTask(task), allowsMultipleExecutions: false);
    }
}
