using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using RunningWater.Controls;
using RunningWater.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RunningWater.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class CalendarViewRowDefinitionConverter : IValueConverter, IMarkupExtension
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ObservableCollection<CalendarDayItemViewModel> collection)
                throw new ArgumentException();

            var rows = new RowDefinitionCollection();

            for (int row = 0; row < collection.Max(day => day.Row); row++)
                rows.Add(new RowDefinition { Height = CalendarMonthView.RowHeight });

            return rows;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();

        /// <inheritdoc/>
        public object ProvideValue(IServiceProvider serviceProvider)
            => this;
    }
}
