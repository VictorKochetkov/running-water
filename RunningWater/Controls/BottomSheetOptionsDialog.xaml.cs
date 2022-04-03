using System.Collections.Generic;
using System.Windows.Input;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using RunningWater.Sources;
using RunningWater.ViewModels;
using Xamarin.Forms;

namespace RunningWater.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class BottomSheetOptionsDialogViewModel : BaseViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<DialogOption> Options { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public partial class BottomSheetOptionsDialog : PopupPage
    {
        /// <summary>
        /// 
        /// </summary>
        public BottomSheetOptionsDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommand Close => new Command(async () =>
        {
            await PopupNavigation.Instance.PopAsync();
        });
    }
}
