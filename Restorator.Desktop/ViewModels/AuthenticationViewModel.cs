using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Domain.Services;
using System.Threading.Tasks;

namespace Restorator.Desktop.ViewModels
{
    public partial class AuthenticationViewModel : ViewModelBase
    {
        private readonly Services.INavigationService _navigationService;
        private readonly SignInViewModel _signInViewModel;
        private readonly SignUpViewModel _signUpViewModel;
        private readonly AccountRestoreViewModel _accountRestoreViewModel;
        public AuthenticationViewModel(Services.INavigationService navigationService,
                                       SignInViewModel signInViewModel,
                                       SignUpViewModel signUpViewModel,
                                       AccountRestoreViewModel accountRestoreViewModel,
                                       ISessionManager sessionManager)
        {
            _navigationService = navigationService;
            _signInViewModel = signInViewModel;
            _signUpViewModel = signUpViewModel;

            NavigateToSignInPage();

            _accountRestoreViewModel = accountRestoreViewModel;

            sessionManager.UserLoggedIn += UserSignedIn;
            sessionManager.UserLoggedOut += UserLoggedOut;
        }

        private void UserLoggedOut()
        {
            Authenticated = false;
        }

        [ObservableProperty]
        private AuthenticationViewModelBase currentViewModel;

        [ObservableProperty]
        private bool authenticated = false;

        [ObservableProperty]
        private IRelayCommand navigateBackCommand;

        private async void UserSignedIn()
        {
            Authenticated = true;

            await NavigateToMenu();
        }

        [RelayCommand]
        public void NavigateToSignUpPage()
        {
            CurrentViewModel = _signUpViewModel;
            NavigateBackCommand = NavigateToSignInPageCommand;
        }

        [RelayCommand]
        public void NavigateToSignInPage()
        {
            CurrentViewModel = _signInViewModel;
            NavigateBackCommand = NavigateToMenuCommand;
        }

        [RelayCommand]
        public void NavigateToPasswordRestorePage()
        {
            CurrentViewModel = _accountRestoreViewModel;
            NavigateBackCommand = NavigateToSignInPageCommand;
        }

        partial void OnAuthenticatedChanged(bool value)
        {
            if (!Authenticated)
                _navigationService.ResetNavigation();
        }

        [RelayCommand]
        public async Task NavigateToMenu()
        {
            if (CurrentViewModel.Role == Domain.Models.Enums.Roles.User)
                await _navigationService.NavigateBackAsync();
            else
                await _navigationService.NavigateAsync<MenuViewModel>();
        }
    }
}