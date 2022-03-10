using RunningWater.ViewModels;
using Xamarin.Forms;

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
