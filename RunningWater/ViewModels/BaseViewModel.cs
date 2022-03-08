using System;
using System.Threading.Tasks;

namespace RunningWater.ViewModels
{
    public class BaseViewModel
    {
        protected virtual Task OnAppearing()
        {
            return Task.FromResult(true);
        }

        protected virtual Task OnDisapearing()
        {
            return Task.FromResult(true);
        }
    }
}
