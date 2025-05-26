using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restorator.Desktop.Models;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Domain.Models.Reservations;
using Restorator.Domain.Services;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace Restorator.Desktop.ViewModels
{
    public partial class RestaurantReservationsManagementViewModel : ViewModelBase
    {
        private readonly IReservationService _reservationService;
        private readonly ISnackbarService _snackbarService;
        private readonly IContentDialogService _contentDialogService;
        private readonly Services.INavigationService _navigationService;
        public RestaurantReservationsManagementViewModel(IReservationService reservationService,
                                                         ISnackbarService snackbarService,
                                                         IContentDialogService contentDialogService,
                                                         Services.INavigationService navigationService,
                                                         ISessionManager sessionManager)
        {
            _reservationService = reservationService;
            _snackbarService = snackbarService;
            _contentDialogService = contentDialogService;
            _navigationService = navigationService;
        }
        private int _restaurantId;

        [ObservableProperty]
        private ObservableCollection<ReservationModel> reservations = [];

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private bool noReservations;

        [RelayCommand]
        public async Task LoadRestaurantReservations(int restaurantId)
        {
            _restaurantId = restaurantId;

            await RefreshRestaurantReservations();
        }

        async partial void OnSelectedDateChanged(DateTime value)
        {
            await RefreshRestaurantReservations();
        }

        [RelayCommand]
        public async Task RefreshRestaurantReservations()
        {
            Reservations.Clear();

            var result = await _reservationService.GetReservations(new GetReservationsDTO()
            {
                SelectedDate =  DateOnly.FromDateTime(SelectedDate),
                RestaurantId = _restaurantId
            });

            if (result.IsFailed)
            {
                _snackbarService.Show("Ошибка", "Что-то пошло не так", Wpf.Ui.Controls.ControlAppearance.Danger);

                return;
            }

            foreach (var reservation in result.Value)
            {
                Reservations.Add(new ReservationModel
                {
                    Id = reservation.Id,
                    Username = reservation.Username,
                    ReservationStart = reservation.ReservationStart,
                    ReservationEnd = reservation.ReservationEnd,
                    Canceled = reservation.Canceled,
                });
            }

            NoReservations = !Reservations.Any();
        }

        [RelayCommand]
        public async Task CancelReservation(ReservationModel reservationInfo)
        {
            var confirm = await _contentDialogService.ShowAsync(new Dialogs.CancelTableReservationDialog(), new CancellationToken());

            if (confirm != Wpf.Ui.Controls.ContentDialogResult.Primary)
                return;

            var result = await _reservationService.CancelReservation(reservationInfo.Id);

            if (result.IsFailed)
            {
                _snackbarService.Show("Ошибка", "Что-то пошло не так", Wpf.Ui.Controls.ControlAppearance.Danger);

                return;
            }

            reservationInfo.Canceled = true;
        }

        [RelayCommand]
        public async Task CloseReservationsManagement() => await _navigationService.NavigateBackAsync();
    }
}