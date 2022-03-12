using System;
using System.Threading.Tasks;
using RunningWater.Pages;

namespace RunningWater.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TPage"></typeparam>
        /// <returns></returns>
        Task NavigateAsync<TPage>() where TPage : BasePage, new();
    }
}
