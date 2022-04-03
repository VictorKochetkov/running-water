using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using RunningWater.Controls;
using RunningWater.Interfaces;
using Xamarin.Forms;

namespace RunningWater.Sources
{
    /// <summary>
    /// 
    /// </summary>
    public class DialogOption
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="command"></param>
        public DialogOption(string title, ICommand command)
        {
            Title = title;
            Command = command;
        }

        /// <summary>
        /// 
        /// </summary>
        public DialogOption()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDestructive { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsBold { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public object CommandParameter { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <inheritdoc/>
        public Task OptionsAsync(params DialogOption[] options)
            => OptionsAsync(null, options);

        /// <inheritdoc/>
        public Task OptionsAsync(string title, IEnumerable<DialogOption> options)
            => OptionsAsync(title, options.ToArray());

        /// <inheritdoc/>
        public Task OptionsAsync(IEnumerable<DialogOption> options)
            => OptionsAsync(null, options.ToArray());

        /// <inheritdoc/>
        public virtual async Task OptionsAsync(string title, params DialogOption[] options)
        {
            PopupPage page = null;

            page = new BottomSheetOptionsDialog()
            {
                BindingContext = new BottomSheetOptionsDialogViewModel()
                {
                    Title = title,
                    Options = options.Select(sourceOption =>
                    {
                        var sourceCommand = sourceOption.Command;

                        sourceOption.Command = new Command(async () =>
                        {
                            await App.Current.MainPage.Navigation.RemovePopupPageAsync(page);
                            sourceCommand?.Execute(null);
                        });
                        return sourceOption;
                    })
                }
            };

            await App.Current.MainPage.Navigation.PushPopupAsync(page);
        }

        /// <inheritdoc/>
        public async Task DisplayAlert(string title, string message, string cancel)
        {
            await App.Current.MainPage.Navigation.NavigationStack.LastOrDefault().DisplayAlert(title, message, cancel);
        }

        /// <inheritdoc/>
        public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return await App.Current.MainPage.Navigation.NavigationStack.LastOrDefault().DisplayAlert(title, message, accept, cancel);
        }
    }
}
