using RunningWater.ViewModels;

namespace RunningWater.Pages
{
    public partial class MainPage : BasePage
    {
        public MainPage()
        {
            InitializeComponent();

            BindingContext = Startup.GetViewModel<MainViewModel>();
        }
    }
}
