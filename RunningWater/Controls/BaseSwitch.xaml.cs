using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace RunningWater.Controls
{
    public partial class BaseSwitch : ContentView
    {
        /// <summary>
        /// Command to be executed when tap on view. Only for internal use.
        /// </summary>
        public ICommand InternalTapCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSwitch"/> class.
        /// </summary>
        public BaseSwitch()
        {
            InternalTapCommand = new Command(() =>
            {
                IsToggled = !IsToggled;
                Command?.Execute(CommandParameter);
            });

            InitializeComponent();
        }

        /// <summary>
        /// Bindable property for <see cref="IsToggled"/>.
        /// </summary>
        public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
            nameof(IsToggled),
            typeof(bool),
            typeof(BaseSwitch),
            false,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: async (bindable, oldValue, newValue) =>
            {
                var view = (BaseSwitch)bindable;

                await Task.WhenAll(
                    view.circle.TranslateTo(view.IsToggled ? view.GetPositionOnToggled() : 0, 0, 120),
                    view.background.FadeTo(view.IsToggled ? 1 : 0, 120));
            });

        /// <summary>
        /// Bindable property for <see cref="Command"/>.
        /// </summary>
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
           nameof(Command),
           typeof(ICommand),
           typeof(BaseSwitch),
           null);

        /// <summary>
        /// Bindable property for <see cref="CommandParameter"/>.
        /// </summary>
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
           nameof(CommandParameter),
           typeof(object),
           typeof(BaseSwitch),
           null);

        /// <summary>
        /// Bindable property for <see cref="IsClickable"/>.
        /// </summary>
        public static readonly BindableProperty IsClickableProperty = BindableProperty.Create(
           nameof(IsClickable),
           typeof(bool),
           typeof(BaseSwitch),
           true);

        /// <summary>
        /// Is toggled.
        /// </summary>
        public bool IsToggled
        {
            get => (bool)GetValue(IsToggledProperty);
            set => SetValue(IsToggledProperty, value);
        }

        /// <summary>
        /// Command to be executed on <see cref="IsToggled"/> changed.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Command parameter.
        /// </summary>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Is clickable.
        /// </summary>
        public bool IsClickable
        {
            get => (bool)GetValue(IsClickableProperty);
            set => SetValue(IsClickableProperty, value);
        }

        /// <summary>
        /// Circle position when state is toggled.
        /// </summary>
        private double GetPositionOnToggled()
           => container.WidthRequest - circle.WidthRequest - (circle.Margin.Left + circle.Margin.Right);
    }
}
