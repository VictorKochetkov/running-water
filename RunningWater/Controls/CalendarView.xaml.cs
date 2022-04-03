using System.Collections.ObjectModel;
using System.Windows.Input;
using RunningWater.ViewModels;
using Xamarin.Forms;

namespace RunningWater.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CalendarView : ContentView
    {
        /// <summary>
        /// Bindable property for <see cref="Months"/>.
        /// </summary>
        public static readonly BindableProperty MonthsProperty = BindableProperty.Create(
            nameof(Months),
            typeof(ObservableCollection<CalendarMonthItemViewModel>),
            typeof(CalendarView),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay);

        /// <summary>
        /// Bindable property for <see cref="Months"/>.
        /// </summary>
        public static readonly BindableProperty DayTapCommandProperty = BindableProperty.Create(
            nameof(DayTapCommand),
            typeof(ICommand),
            typeof(CalendarView),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay);

        /// <summary>
        /// 
        /// </summary>
        public CalendarView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<CalendarMonthItemViewModel> Months
        {
            get => (ObservableCollection<CalendarMonthItemViewModel>)GetValue(MonthsProperty);
            set => SetValue(MonthsProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommand DayTapCommand
        {
            get => (ICommand)GetValue(DayTapCommandProperty);
            set => SetValue(DayTapCommandProperty, value);
        }


    }
}
