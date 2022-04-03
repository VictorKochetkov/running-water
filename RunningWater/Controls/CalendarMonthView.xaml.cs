using System;
using Xamarin.Forms;

namespace RunningWater.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CalendarMonthView : StackLayout
    {
        /// <summary>
        /// 
        /// </summary>
        public CalendarMonthView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        private static double ViewWidth;

        /// <summary>
        /// 
        /// </summary>
        public static double DefaultSize = 36;

        /// <summary>
        /// 
        /// </summary>
        public static double ColumnWidth => Math.Max(DefaultSize, (ViewWidth - ParentPadding) / 7);

        /// <summary>
        /// 
        /// </summary>
        public static double RowHeight => Math.Min(ColumnWidth, DefaultSize);

        /// <summary>
        /// 
        /// </summary>
        private static double ParentPadding = 20;

        /// <inheritdoc/>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width <= 0 || ViewWidth == width)
                return;

            ViewWidth = width;

            OnPropertyChanged(nameof(ColumnWidth));
            OnPropertyChanged(nameof(RowHeight));
        }
    }
}
