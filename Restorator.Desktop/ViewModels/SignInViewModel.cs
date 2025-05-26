using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Domain.Models.Account;
using Restorator.Domain.Models.Enums;
using Restorator.Domain.Services;
using System.ComponentModel.DataAnnotations;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace Restorator.Desktop.ViewModels
{
    public partial class SignInViewModel : AuthenticationViewModelBase
    {
        private readonly IAccountService _authenticationService;
        private readonly ISessionManager _sessionManager;
        private readonly ISnackbarService _snackbarService;
        public SignInViewModel(IAccountService authenticationService,
                               ISessionManager sessionManager,
                               ISnackbarService snackbarService)
        {
            _authenticationService = authenticationService;
            _sessionManager = sessionManager;
            _snackbarService = snackbarService;
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Логин и пароль обязательны для заполнения")]
        [ObservableProperty]
        private string login;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Логин и пароль обязательны для заполнения")]
        [ObservableProperty]
        private string password;

        [RelayCommand]
        public async Task SignIn()
        {
            ValidateAllProperties();

            if (HasErrors)
            {
                var error = GetErrors().First();

                _snackbarService.Show("Так не пойдет", error.ErrorMessage, Wpf.Ui.Controls.ControlAppearance.Danger);

                return;
            }

            var result = await _authenticationService.SignInAsync(new SignInDTO()
            {
                Login = Login,
                Password = Password
            });

            if (!result.IsSuccess)
            {
                _snackbarService.Show("Ошибка", "Кажется такого пользователя нет", Wpf.Ui.Controls.ControlAppearance.Danger);

                return;
            }

            Role = Enum.Parse<Roles>(result.Value.SessionInfo.Role);

            _snackbarService.Show("С возвращением", "Мы рады видеть тебя снова", Wpf.Ui.Controls.ControlAppearance.Success);
        }
    }
}
