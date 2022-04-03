using System;
using ReactiveUI.Fody.Helpers;

namespace RunningWater.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class CalendarDayItemViewModel : BaseViewModel
    {
        public CalendarDayItemViewModel(DateTime date, int column, int row)
        {
            Date = date;
            Column = column;
            Row = row;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Day => $"{Date.Day}";

        /// <summary>
        /// 
        /// </summary>
        public bool IsToday => Date.Date == DateTime.Now.Date;

        /// <summary>
        /// 
        /// </summary>
        public bool IsEnabled => Date.Date >= DateTime.Now.Date;

        /// <summary>
        /// 
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// 
        /// </summary>
        [Reactive]
        public bool IsSelected { get; set; }
    }
}
