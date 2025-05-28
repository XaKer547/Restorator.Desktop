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
    public partial class SignUpViewModel : AuthenticationViewModelBase
    {
        private readonly IAccountService _authenticationService;
        private readonly ISessionManager _sessionManager;
        private readonly ISnackbarService _snackbarService;
        public SignUpViewModel(IAccountService authenticationService,
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

        [Required(AllowEmptyStrings = false, ErrorMessage = "Почта обязательна для заполнения")]
        [ObservableProperty]
        private string email;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Логин и пароль обязательны для заполнения")]
        [ObservableProperty]
        private string password;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Нам нужно знать как к вам обращаться")]
        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private Roles role = Roles.User;

        [RelayCommand]
        public async Task SignUp()
        {
            ValidateAllProperties(); // допустим я хочу проверить этот участок кода 

            if (HasErrors) // когда исполнение кода дойдет до сюда, сработает остановка
            { // на переменные и свойства можно наводить мышь, чтобы смотреть их содержимое
                var error = GetErrors().First();

                _snackbarService.Show("Так не пойдет", error.ErrorMessage, Wpf.Ui.Controls.ControlAppearance.Danger);

                return;
            }

            if (!ValidatePassword())
            {
                _snackbarService.Show("Так не пойдет", "В пароле должна быть хотя-бы одна цифра", Wpf.Ui.Controls.ControlAppearance.Danger);

                return;
            }

            var signUnDto = new SignUpDTO()
            {
                Login = Login,
                RoleId = (int)Role,
                Username = Username,
                Password = Password,
                Email = Email
            };

            var result = await _authenticationService.SignUpAsync(signUnDto);

            if (result.IsFailed)
            {
                if (result.Errors.Count != 0)
                    _snackbarService.Show("Упс", result.Errors[0].Message, Wpf.Ui.Controls.ControlAppearance.Danger);
                else
                    _snackbarService.Show("Упс", "У нас не получилось тебя зарегистрировать, попробуй чуть позже", Wpf.Ui.Controls.ControlAppearance.Danger);

                return;
            }
            //get token

            var signInResult = await _authenticationService.SignInAsync(new SignInDTO()
            {
                Login = Login,
                Password = Password
            });

            var session = signInResult.Value;

            Role = Enum.Parse<Roles>(session.SessionInfo.Role);

            _snackbarService.Show("Добро пожаловать в семью", "Let's celebrate and eat some chick", Wpf.Ui.Controls.ControlAppearance.Success);
        }

        private bool ValidatePassword()
        {
            return Password.Any(p => char.IsDigit(p));
        }


        [RelayCommand]
        public void ChangeSelectedRole(Roles role)
        {
            Role = role;
        }
    }
}