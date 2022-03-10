using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace RunningWater.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public enum ViewModelState
    {
        Default,
        MinorLoading,
        Loading,
    }

    /// <summary>
    /// 
    /// </summary>
    public class BaseViewModel : ReactiveObject
    {
        /// <summary>
        /// 
        /// </summary>
        [Reactive]
        public ViewModelState State { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsBusy => State == ViewModelState.Loading || State == ViewModelState.MinorLoading;

        /// <summary>
        /// 
        /// </summary>
        public BaseViewModel()
        {
            this.WhenAnyValue(viewModel => viewModel.State)
                .Subscribe(value => OnPropertyChanged(nameof(IsBusy)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => this.RaisePropertyChanged(propertyName);

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
        /// <param name="task"></param>
        /// <param name="minorLoading"></param>
        /// <param name="loading"></param>
        /// <returns></returns>
        protected async Task DoTask(Func<Task> task, bool minorLoading = true, bool loading = false)
        {
            if (minorLoading)
                State = ViewModelState.MinorLoading;

            if (loading)
                State = ViewModelState.Loading;

            try
            {
                await task();
            }
            catch (Exception exception)
            {
#if DEBUG
                Debugger.Break();
#endif
                Console.WriteLine(exception);
            }

            if (minorLoading || loading)
                State = ViewModelState.Default;
        }
    }
}
