using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Domain.Models.Account;
using Restorator.Domain.Models.Enums;
using Restorator.Domain.Services;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace Restorator.Desktop.ViewModels
{
    public partial class AccountRestoreViewModel : AuthenticationViewModelBase
    {
        private readonly IAccountService _accountService;
        private readonly ISnackbarService _snackbarService;
        public AccountRestoreViewModel(IAccountService accountService,
                                       ISnackbarService snackbarService)
        {
            _accountService = accountService;
            _snackbarService = snackbarService;
        }

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private bool requestSent = false;

        [ObservableProperty]
        private string otp;

        [ObservableProperty]
        private bool signedIn = false;

        [ObservableProperty]
        private string newPassword;

        [RelayCommand]
        public void ResetViewModelState()
        {
            Email = string.Empty;
            RequestSent = false;
            Otp = string.Empty;
            SignedIn = false;
            NewPassword = string.Empty;
        }

        [RelayCommand]
        public async Task SendPasswordRestoreRequest()
        {
            var result = await _accountService.RequestPasswordReset(Email);

            if (result.IsFailed)
            {
                _snackbarService.Show("Ошибка", "Кажется такого пользователя нет", Wpf.Ui.Controls.ControlAppearance.Danger);

                return;
            }

            RequestSent = true;
        }

        [RelayCommand]
        public async Task SingInViaOtp()
        {
            var result = await _accountService.SignInAsync(new RecoverAccountDTO
            {
                Email = Email,
                OTP = Otp
            });

            if (result.IsFailed)
            {
                _snackbarService.Show("Ошибка", "Кажется вы ввели не верный OTP код", Wpf.Ui.Controls.ControlAppearance.Danger);

                //fuck

                return;
            }

            SignedIn = true;

            Role = Enum.Parse<Roles>(result.Value.SessionInfo.Role);
        }

        [RelayCommand]
        public async Task ChangePassword()
        {
            var result = await _accountService.UpdatePassword(NewPassword);

            if (result.IsFailed)
            {
                _snackbarService.Show("Ошибка", "Не удалось изменить пароль", Wpf.Ui.Controls.ControlAppearance.Danger);

                return;
            }

            _snackbarService.Show("С возвращением", "Мы рады видеть тебя снова", Wpf.Ui.Controls.ControlAppearance.Success);
        }
    }
}