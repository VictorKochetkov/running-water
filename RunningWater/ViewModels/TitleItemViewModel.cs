namespace RunningWater.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class TitleItemViewModel : BaseViewModel
    {
        public TitleItemViewModel(string title)
            => Title = title;

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; }
    }
}
