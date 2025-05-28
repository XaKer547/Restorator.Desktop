using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restorator.Desktop.Dialogs;
using Restorator.Desktop.Models;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Domain.Models.Reservations;
using Restorator.Domain.Models.Restaurant;
using Restorator.Domain.Services;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace Restorator.Desktop.ViewModels
{
    public partial class RestaurantReservationViewModel : RestaurantViewModelBase
    {
        private readonly IReservationService _reservationService;
        private readonly IContentDialogService _contentDialogService;
        private readonly ISnackbarService _snackbarService;
        private readonly Services.INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<TableModel> _tables = [];

        [ObservableProperty]
        private ReservationInfoDTO reservationInfo;

        [ObservableProperty]
        private bool reservationInfoShow;

        [ObservableProperty]
        private bool canCancelReservation;

        [ObservableProperty]
        private string plan;

        [ObservableProperty]
        private DateTime reservationStartTime;

        [ObservableProperty]
        private DateTime reservationEndTimeLimit;

        [ObservableProperty]
        private DateTime reservationEndTime;

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private bool isToday = true;

        [ObservableProperty]
        private bool canSearchReserve = true;

        [ObservableProperty]
        private int hours;

        [ObservableProperty]
        private DateTime beginWorkTime;

        private DateTime _beginWorkTimeBuffer;

        [ObservableProperty]
        private DateTime endWorkTime;

        public RestaurantReservationViewModel(IContentDialogService contentDialogService,
                                    IReservationService reservationService,
                                    Services.INavigationService navigationService,
                                    ISnackbarService snackbarService)
        {
            _contentDialogService = contentDialogService;
            _reservationService = reservationService;
            _navigationService = navigationService;
            _snackbarService = snackbarService;
        }

        private int _restaurantId;

        public async Task LoadRestaurantPlan(int restaurantId, TimeOnly beginWorkTime, TimeOnly endWorkTime)
        {
            _restaurantId = restaurantId;

            _beginWorkTimeBuffer = SelectedDate.Add(beginWorkTime.ToTimeSpan());

            UpdateBeginWorkTime();

            var endTimeBuffer = SelectedDate.Add(endWorkTime.ToTimeSpan());

            if (EndWorkTime <= BeginWorkTime)
                endTimeBuffer = endTimeBuffer.AddDays(1);

            EndWorkTime = endTimeBuffer;

            reservationStartTime = BeginWorkTime;
            reservationEndTime = BeginWorkTime.AddHours(1);

            CheckReservationSearchAvaibility();

            var result = await _reservationService.GetRestaurantReservationPlan(BuildRestaurantPlanQuery());
            //searchs null

            if (result.IsFailed)
            {
                await _navigationService.NavigateBackAsync();

                _snackbarService.Show("Ой", "Что-то пошло не так", ControlAppearance.Danger);

                return;
            }

            var value = result.Value;

            Plan = value.Scheme;

            var tables = value.Tables.Select(t => new TableModel
            {
                Id = t.Id,
                ReservationId = t.ReservationId,
                X = t.X,
                Y = t.Y,
                Width = t.Width,
                Height = t.Height,
                Rotation = t.Rotation,
                State = t.State
            });

            foreach (var table in tables)
                Tables.Add(table);
        }

        private bool _waitingRefresh;
        async partial void OnSelectedDateChanged(DateTime value)
        {
            IsToday = value == DateTime.Today;

            var daysPast = SelectedDate.Day - ReservationStartTime.Day;

            await UpdateWorkTime();

            _waitingRefresh = true;

            ReservationEndTime = ReservationEndTime.AddDays(daysPast);
            ReservationStartTime = ReservationStartTime.AddDays(daysPast);

            _waitingRefresh = false;

            await RefreshReservationPlan();
        }

        private async Task UpdateWorkTime()
        {
            _waitingRefresh = true;

            _beginWorkTimeBuffer = SelectedDate.Add(BeginWorkTime.TimeOfDay);

            UpdateBeginWorkTime();

            var endTimeBuffer = SelectedDate.Add(EndWorkTime.TimeOfDay);

            if (EndWorkTime <= BeginWorkTime)
                endTimeBuffer = endTimeBuffer.AddDays(1);

            EndWorkTime = endTimeBuffer;

            await RefreshReservationPlan();

            _waitingRefresh = false;
        }

        async partial void OnReservationStartTimeChanged(DateTime value)
        {
            if (ReservationStartTime >= ReservationEndTime)
            {
                _waitingRefresh = true;

                ReservationEndTime = ReservationStartTime.AddHours(1);

                _waitingRefresh = false;

                return;
            }

            if (ReservationStartTime < BeginWorkTime)
            {
                ReservationStartTime = BeginWorkTime;
                return;
            }

            if (!_waitingRefresh)
                await RefreshReservationPlan();
        }

        async partial void OnReservationEndTimeChanged(DateTime value)
        {
            var onNewDay = ReservationEndTime.Day > ReservationStartTime.Day;

            if (ReservationEndTime > EndWorkTime)
            {
                _waitingRefresh = true;

                ReservationEndTime = EndWorkTime;

                _waitingRefresh = false;

                return;
            }

            if (value.Hour < ReservationStartTime.Hour && !onNewDay)
            {
                _waitingRefresh = true;

                ReservationEndTime = ReservationEndTime.AddDays(1);

                _waitingRefresh = false;

                return;
            }
            else if (value.Hour > ReservationStartTime.Hour && onNewDay)
            {
                _waitingRefresh = true;

                ReservationEndTime = ReservationEndTime.AddDays(-1);

                _waitingRefresh = false;

                return;
            }

            if (!onNewDay && ReservationStartTime > ReservationEndTime)
            {
                _waitingRefresh = true;

                ReservationEndTime = ReservationStartTime;

                _waitingRefresh = false;

                return;
            }

            if (!_waitingRefresh)
                await RefreshReservationPlan();
        }

        partial void OnIsTodayChanged(bool value)
        {
            UpdateBeginWorkTime();

            CheckReservationSearchAvaibility();
        }

        private void UpdateBeginWorkTime()
        {
            if (IsToday)
            {
                var date = DateOnly.FromDateTime(DateTime.Now);

                var time = TimeOnly.FromDateTime(DateTime.Now);

                BeginWorkTime = new DateTime(date, new TimeOnly(time.Hour, time.Minute), DateTimeKind.Unspecified);
            }
            else
                BeginWorkTime = _beginWorkTimeBuffer;
        }

        private void CheckReservationSearchAvaibility()
        {
            if (!IsToday)
                return;

            CanSearchReserve = (IsToday && BeginWorkTime < EndWorkTime) ^ !IsToday;
        }

        [RelayCommand(CanExecute = nameof(CanSearchReserve), AllowConcurrentExecutions = false)]
        public async Task RefreshReservationPlan()
        {
            ReservationInfoShow = false;

            Tables.Clear();
            _reservedTables.Clear();

            var result = await _reservationService.GetRestaurantReservationPlan(BuildRestaurantPlanQuery());

            if (result.IsFailed)
            {
                await _navigationService.NavigateBackAsync();

                _snackbarService.Show("Ой", "Что-то пошло не так", ControlAppearance.Danger);

                return;
            }

            var tables = result.Value.Tables.Select(t => new TableModel
            {
                Id = t.Id,
                ReservationId = t.ReservationId,
                X = t.X,
                Y = t.Y,
                Width = t.Width,
                Height = t.Height,
                Rotation = t.Rotation,
                State = t.State,
            });

            foreach (var table in tables)
                Tables.Add(table);
        }

        private GetRestaurantPlanDTO BuildRestaurantPlanQuery()
        {
            return new GetRestaurantPlanDTO()
            {
                ReservationStartDate = ReservationStartTime,
                ReservationEndDate = ReservationEndTime,
                RestaurantId = _restaurantId,
            };
        }


        private readonly HashSet<int> _reservedTables = [];
        [RelayCommand(CanExecute = nameof(CanSearchReserve))]
        public async Task GetTableReservationInfo(TableModel table)
        {
            if (!table.ReservationId.HasValue)
            {
                ReservationInfoShow = false;

                if (table.State == Domain.Models.Enums.TableStates.PendingReservation)
                {
                    _reservedTables.Remove(table.Id);
                    table.State = Domain.Models.Enums.TableStates.Avaible;

                    return;
                }

                _reservedTables.Add(table.Id);
                table.State = Domain.Models.Enums.TableStates.PendingReservation;

                return;
            }

            var info = await _reservationService.GetReservationInfo(table.ReservationId!.Value);

            ReservationInfo = info.Value;

            CanCancelReservation = table.State == Domain.Models.Enums.TableStates.OccupiedByUser;

            ReservationInfoShow = true;
        }

        [RelayCommand(CanExecute = nameof(CanCancelReservation))]
        public async Task CancelReservation()
        {
            var confirm = await _contentDialogService.ShowSimpleDialogAsync(new SimpleContentDialogCreateOptions()
            {
                Title = "Отмена бронирования стола",
                Content = "Вы действительно хотите отменить бронь?",
                PrimaryButtonText = "Да, хочу",
                CloseButtonText = "Нет, я передумал"
            });

            if (confirm != ContentDialogResult.Primary)
                return;

            var cancelResult = await _reservationService.CancelReservation(ReservationInfo.Id);

            if (cancelResult.IsFailed)
            {
                _snackbarService.Show("Ой", "Что-то пошло не так", ControlAppearance.Danger);
            }
        }

        [RelayCommand(CanExecute = nameof(CanSearchReserve))]
        public async Task ConfirmTableReservation()
        {
            if (_reservedTables.Count == 0)
            {
                _snackbarService.Show("Так не пойдет", "Вы должны выбрать хотя-бы один стол для бронирования", ControlAppearance.Danger);
                return;
            }

            var dialog = new ConfirmReservationReservationContentDialog(new ConfirmRestaurantReservationModel()
            {
                ReservationStart = TimeOnly.FromDateTime(ReservationStartTime),
                ReservationEnd = TimeOnly.FromDateTime(ReservationEndTime),
                TablesCount = _reservedTables.Count,
            });

            var confirmation = await _contentDialogService.ShowAsync(dialog, new CancellationToken());

            if (confirmation != ContentDialogResult.Primary)
                return;

            var result = await _reservationService.CreateReservation(new CreateRestaurantReservationDTO
            {
                RestaurantId = _restaurantId,
                ReservedTables = _reservedTables,
                ReservationStartDate = ReservationStartTime,
                ReservationEndDate = ReservationEndTime,
            });

            if (result.IsFailed)
            {
                _snackbarService.Show("Ой", "Что-то пошло не так", ControlAppearance.Danger);

                return;
            }

            _reservedTables.Clear();

            _snackbarService.Show("Ура!", "Будем ждать вас к назначенному времени!", ControlAppearance.Success);

            await RefreshReservationPlan();
        }


        [RelayCommand]
        public async Task CloseRestaurantReservation() => await _navigationService.NavigateBackAsync();
    }
}