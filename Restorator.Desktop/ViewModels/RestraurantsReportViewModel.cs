using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Domain.Models.Reports;
using Restorator.Domain.Models.Restaurant;
using Restorator.Domain.Services;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace Restorator.Desktop.ViewModels
{
    public partial class RestraurantsReportViewModel : ViewModelBase
    {
        private readonly IReportService _reportService;
        private readonly IRestaurantService _restaurantService;
        public RestraurantsReportViewModel(IReportService reportService, IRestaurantService restaurantService)
        {
            _reportService = reportService;

            Months = [.. System.Globalization.DateTimeFormatInfo.CurrentInfo.MonthNames
                .Where(m => m != string.Empty)
                .Select((m, i) => new MonthDTO
                {
                    Name = m,
                    Index = i + 1
                })];
            SelectedMonth = Months.Single(x => x.Index == SelectedDate.Month);

            Years = Enumerable.Range(1930, SelectedDate.Year - 1930 + 1);
            SelectedYear = SelectedDate.Year;
            _restaurantService = restaurantService;
        }


        [ObservableProperty]
        private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Today);

        [ObservableProperty]
        private ISeries[] _reservationsSeries;

        [ObservableProperty]
        private ISeries[] _reservationDaysSeries;

        [ObservableProperty]
        private IEnumerable<MonthDTO> months;

        [ObservableProperty]
        private MonthDTO selectedMonth;

        [ObservableProperty]
        private IEnumerable<int> years;

        [ObservableProperty]
        private int selectedYear;

        [ObservableProperty]
        private ObservableCollection<ISeries[]> _restaurantSplitSeries;

        [ObservableProperty]
        private ICartesianAxis[] days = [
            new Axis
            {
                Labels = [
                    "ПН",
                    "ВТ",
                    "СР",
                    "ЧТ",
                    "ПТ",
                    "СБ",
                    "ВС",
                ],
            }
        ];

        [ObservableProperty]
        private float canceledPercentRate;

        [ObservableProperty]
        private MonthSummaryReportDTO monthReport;

        [ObservableProperty]
        private IReadOnlyCollection<RestaurantSearchItemDTO> restaurants;

        [ObservableProperty]
        private RestaurantSearchItemDTO? selectedRestaurant;

        [ObservableProperty]
        private bool showingSummary;

        private int? selectedRestaurantId;

        //[ObservableProperty]
        //public bool isEmpty;

        //[ObservableProperty]
        //public int waitingReservations;

        //[ObservableProperty]
        //public int finishedReservations;

        //[ObservableProperty]
        //public int canceledReservations;

        //[ObservableProperty]
        //public IEnumerable<RestaurantDailyReservationReportDTO> restaurantDailyReservations;

        //[ObservableProperty]
        //public string mostPopularDay;

        //[ObservableProperty]
        //public MonthPopularRestaurantReportDTO mostPopularRestaurant;

        //[ObservableProperty]
        //public ReservationsRateReportDTO reservationsRate;


        [RelayCommand]
        public async Task Initialize()
        {
            Restaurants = await _restaurantService.GetOwnedRestaurantsSearchItems();

            await LoadMonthStatistics();

            Initialized = true;
        }

        partial void OnSelectedYearChanged(int value)
        {
            if (!Initialized)
                return;

            UpdateSelectedDate();
        }
        partial void OnSelectedMonthChanged(MonthDTO value)
        {
            if (!Initialized)
                return;

            UpdateSelectedDate();
        }

        private void UpdateSelectedDate()
        {
            SelectedDate = new DateOnly(SelectedYear, SelectedMonth.Index, 1);
        }

        async partial void OnSelectedDateChanged(DateOnly value)
        {
            await LoadMonthStatistics();
        }

        async partial void OnSelectedRestaurantChanged(RestaurantSearchItemDTO value)
        {
            if (SelectedRestaurant is null)
                selectedRestaurantId = null;
            else
                selectedRestaurantId = SelectedRestaurant.Id;

            await LoadMonthStatistics();
        }

        [RelayCommand]
        public async Task LoadMonthStatistics()
        {
            MonthReport = await _reportService.GetMonthSummaryReport(SelectedDate, selectedRestaurantId);

            if (MonthReport.IsEmpty)
                return;

            CalculateCanceledPercent();

            FillRestraurantSplitSeries();
        }

        [RelayCommand]
        public void RemoveSelectedRestaurant()
        {
            SelectedRestaurant = null;
        }


        private void CalculateCanceledPercent()
        {
            if (MonthReport.Canceled == 0)
            {
                CanceledPercentRate = 0;
                return;
            }

            var total = MonthReport.Reserved + MonthReport.Finished + MonthReport.Canceled;

            var percent = 100f / (total / MonthReport.Canceled);

            CanceledPercentRate = (float)Math.Round(percent, 2);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
        private void FillRestraurantSplitSeries()
        {
            ReservationsSeries =
            [
                new PieSeries<int>
                {
                    Name = "Ожидаются",
                    Values = [MonthReport.Reserved],
                    Stroke = new SolidColorPaint(SKColors.Blue),
                    Fill = new SolidColorPaint(SKColors.Blue)
                },
                new PieSeries<int>
                {
                    Name = "Отменено",
                    Values = [MonthReport.Canceled],
                    Stroke = new SolidColorPaint(SKColors.Red),
                    Fill = new SolidColorPaint(SKColors.Red)
                },
                new PieSeries<int>
                {
                    Name = "Успешные",
                    Values = [MonthReport.Finished],
                    Stroke = new SolidColorPaint(SKColors.LimeGreen),
                    Fill = new SolidColorPaint(SKColors.LimeGreen)
                },
            ];

            ReservationDaysSeries = [.. MonthReport.RestaurantDailyReservations.Select(x => new LineSeries<int>()
            {
                Name = x.RestaurantName,
                Values = [.. x.Reservations]
            }).Cast<ISeries>()];
        }


        public class MonthDTO
        {
            public int Index { get; set; }
            public string Name { get; set; }
        }


        //one day...
        private string[] DayNames =
        [
            "Понедельник",
            "Вторник",
            "Среда",
            "Четверг",
            "Пятница",
            "Суббота",
            "Воскресенье",
        ];
    }
}