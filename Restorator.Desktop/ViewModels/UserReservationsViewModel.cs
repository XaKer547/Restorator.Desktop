using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Desktop.Views.Pages;
using Restorator.Domain.Models.Reservations;
using Restorator.Domain.Services;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace Restorator.Desktop.ViewModels
{
    public partial class UserReservationsViewModel : ViewModelBase
    {
        private readonly IReservationService _reservationService;
        private readonly ISnackbarService _snackbarService;
        private readonly IContentDialogService _contentDialogService;
        private readonly INavigationService _navigationService;
        public UserReservationsViewModel(IReservationService reservationService,
                                         ISessionManager sessionManager,
                                         ISnackbarService snackbarService,
                                         IContentDialogService contentDialogService,
                                         INavigationService navigationService)
        {
            _reservationService = reservationService;
            _snackbarService = snackbarService;
            _contentDialogService = contentDialogService;

            _navigationService = navigationService;
        }

        [ObservableProperty]
        private ObservableCollection<ReservationInfoDTO> reservations = [];

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private bool noReservations;

        async partial void OnSelectedDateChanged(DateTime value)
        {
            await LoadReservationHistory();
        }

        [RelayCommand]
        public async Task LoadReservationHistory()
        {
            Reservations.Clear();

            var result = await _reservationService.GetOwnedReservations(new GetOwnedReservationsDTO()
            {
                SelectedDate = DateOnly.FromDateTime(SelectedDate),
            });

            if (result.IsFailed)
            {
                _snackbarService.Show("Ошибка", "Что-то пошло не так", Wpf.Ui.Controls.ControlAppearance.Danger);

                return;
            }

            foreach (var reservation in result.Value)
                Reservations.Add(reservation);

            NoReservations = !Reservations.Any();
        }

        [RelayCommand]
        public void OpenSearch() => _navigationService.Navigate(typeof(RestaurantSearchPage));

        [RelayCommand]
        public async Task CancelReservation(ReservationInfoDTO reservationInfo)
        {
            if (!reservationInfo.CanCancel)
            {
                _snackbarService.Show("Так не пойдет", "Нельзя отменить бронирование, которое уже прошло", Wpf.Ui.Controls.ControlAppearance.Danger);

                return;
            }

            var confirm = await _contentDialogService.ShowAsync(new Dialogs.CancelTableReservationDialog(), new CancellationToken());

            if (confirm != Wpf.Ui.Controls.ContentDialogResult.Primary)
                return;

            var result = await _reservationService.CancelReservation(reservationInfo.Id);

            if (result.IsFailed)
            {
                _snackbarService.Show("Ошибка", "Что-то пошло не так", Wpf.Ui.Controls.ControlAppearance.Danger);

                return;
            }

            Reservations.Remove(reservationInfo);

            _snackbarService.Show("Отмена прошла успешно", "Надеемся, что вы вернетесь", Wpf.Ui.Controls.ControlAppearance.Success);
        }
    }
}
