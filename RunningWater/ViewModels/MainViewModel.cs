using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI.Fody.Helpers;
using RunningWater.Helpers;
using RunningWater.Interfaces;
using RunningWater.Sources;
using RunninWater.Resources;
using Xamarin.Forms;

namespace RunningWater.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class MainViewModel : BasePageViewModel
    {
        private readonly IApiClient _apiClient;

        /// <summary>
        /// Command to be executed when watering state toggle was changed.
        /// </summary>
        public ICommand StateChangedCommand { get; }

        /// <summary>
        /// Command to be executed when `Add` chip tapped.
        /// </summary>
        public ICommand AddTimeTapCommand { get; }

        /// <summary>
        /// Command to be executed when time chip tapped.
        /// </summary>
        public ICommand TimeTapCommand { get; }

        /// <summary>
        /// Command to be executed when calendar day tapped.
        /// </summary>
        public ICommand CalendarDayTapCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigationService"></param>
        /// <param name="dialogService"></param>
        /// <param name="apiClient"></param>
        public MainViewModel(
            INavigationService navigationService,
            IDialogService dialogService,
            IApiClient apiClient)
            : base(navigationService, dialogService)
        {
            _apiClient = apiClient;

            StateChangedCommand = BuildTaskCommand(() =>
            {
                return apiClient.StateWriteAsync(IsEnabled);
            });

            AddTimeTapCommand = BuildTaskCommand(() =>
            {
                return Dialog.OptionsAsync(BuildOptions());
            });

            TimeTapCommand = BuildCommand<TimeItemViewModel>(viewModel =>
            {
                return Dialog.OptionsAsync(new DialogOption
                {
                    Title = "MainPage.Time.Remove".GetString(),
                    IsDestructive = true,
                    Command = new Command(() => TimeCollection.Remove(viewModel)),
                });
            });

            CalendarDayTapCommand = BuildCommand<CalendarDayItemViewModel>(viewModel =>
            {
                viewModel.IsSelected = !viewModel.IsSelected;
                return SaveScheduleAsync();
            });

            MonthsCollection.Add(new CalendarMonthItemViewModel(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)));
            MonthsCollection.Add(new CalendarMonthItemViewModel(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1)));
            MonthsCollection.Add(new CalendarMonthItemViewModel(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(2)));
        }

        /// <summary>
        /// 
        /// </summary>
        public string PlantEmoji => new string[] { "🪴", "🌵", "🌿" }.ElementAt(new Random().Next(0, 3));

        /// <summary>
        /// 
        /// </summary>
        [Reactive]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Reactive]
        public ObservableCollection<CalendarMonthItemViewModel> MonthsCollection { get; private set; } = new ObservableCollection<CalendarMonthItemViewModel>();

        /// <summary>
        /// 
        /// </summary>
        [Reactive]
        public ObservableCollection<BaseViewModel> TimeCollection { get; private set; } = new ObservableCollection<BaseViewModel>
        {
            new TitleItemViewModel("MainPage.Time.Add".GetString()),
        };

        /// <inheritdoc/>
        public override void Prepare(params object[] arguments)
        {
            base.Prepare(arguments);

            IsEnabled = (bool)arguments.ElementAt(0);

            var jobs = (IEnumerable<DateTimeOffset>)arguments.ElementAt(1);

            foreach (var time in jobs.Select(job => job.TimeOfDay).Distinct())
            {
                AddTime(time);
            }

            foreach (var job in jobs)
            {
                foreach (var month in MonthsCollection)
                {
                    if (month.DaysCollection.FirstOrDefault(day => day.Date == job.Date) is { } day)
                        day.IsSelected = true;
                }
            }

            TimeCollection.CollectionChanged += async (s, e) => await SaveScheduleAsync();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Task SaveScheduleAsync() => DoTask(() =>
        {
            var times = TimeCollection
                .Where(viewModel => viewModel is TimeItemViewModel)
                .Cast<TimeItemViewModel>()
                .Select(viewModel => viewModel.Time)
                .ToList();

            var jobs = SelectedCollection
                .Select(viewModel => BuildJobs(viewModel.Date, times))
                .SelectMany(jobs => jobs)
                .ToList();

            if (!jobs.Any())
                return Task.FromResult(true);

            return _apiClient.JobsWriteAsync(jobs);
        });


        /// <summary>
        /// 
        /// </summary>
        private IEnumerable<CalendarDayItemViewModel> SelectedCollection => MonthsCollection
            .Select(month => month.DaysCollection.Where(day => day.IsSelected))
            .SelectMany(days => days);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        private IEnumerable<DateTimeOffset> BuildJobs(DateTime date, IEnumerable<TimeSpan> times)
        {
            foreach (var time in times)
                yield return date.Add(time);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<DialogOption> BuildOptions()
        {
            for (var hour = 10; hour <= 18; hour += 2)
            {
                var time = TimeSpan.FromHours(hour);

                yield return new DialogOption
                {
                    Title = time.ToLocalizedString(),
                    IsDisabled = TimeCollection.Any(viewModel => viewModel is TimeItemViewModel timeViewModel && timeViewModel.Time == time),
                    Command = new Command(() => AddTime(time)),
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        private void AddTime(TimeSpan time)
        {
            var index = TimeCollection.LastOrDefault(viewModel => viewModel is TimeItemViewModel timeViewModel && timeViewModel.Time < time) is { } target
                ? TimeCollection.IndexOf(target) + 1 : 0;

            TimeCollection.Insert(index, new TimeItemViewModel(time));
        }
    }
}
