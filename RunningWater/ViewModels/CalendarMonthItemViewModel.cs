using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace RunningWater.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class CalendarMonthItemViewModel : BaseViewModel
    {
        public CalendarMonthItemViewModel(DateTime date)
        {
            Date = date;
            DaysCollection = new ObservableCollection<CalendarDayItemViewModel>(BuildDays());
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<CalendarDayItemViewModel> DaysCollection { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<CalendarDayItemViewModel> BuildDays()
        {
            int previousColumn = 0;
            int row = 0;

            for (int day = 1; day <= DateTime.DaysInMonth(Date.Year, Date.Month); day++)
            {
                var date = new DateTime(Date.Year, Date.Month, day);

                var column = GetDayOfWeek(date.DayOfWeek);

                if (previousColumn == 6 && column == 0)
                    row++;

                previousColumn = column;

                yield return new CalendarDayItemViewModel(date, GetDayOfWeek(date.DayOfWeek), row);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        private static int GetDayOfWeek(DayOfWeek day) => day switch
        {
            DayOfWeek.Monday => 0,
            DayOfWeek.Tuesday => 1,
            DayOfWeek.Wednesday => 2,
            DayOfWeek.Thursday => 3,
            DayOfWeek.Friday => 4,
            DayOfWeek.Saturday => 5,
            DayOfWeek.Sunday => 6,
            _ => throw new NotImplementedException(),
        };
    }
}
