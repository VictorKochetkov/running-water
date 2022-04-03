using RunningWater.ViewModels;

namespace RunningWater.Pages
{
    public partial class StartPage : BasePage
    {
        public StartPage()
        {
            InitializeComponent();

            BindingContext = App.GetViewModel<StartViewModel>();
        }
    }
}
