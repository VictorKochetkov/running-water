using System;
using Xamarin.Forms;

namespace RunningWater.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class TimeItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TimeTemplate { get; set; }
        public DataTemplate TitleTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is TitleItemViewModel)
                return TitleTemplate;

            if (item is TimeItemViewModel)
                return TimeTemplate;

            throw new NotSupportedException();
        }
    }
}
