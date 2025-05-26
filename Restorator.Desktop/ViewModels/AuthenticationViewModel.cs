using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Domain.Services;

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
                                       AccountRestoreViewModel accountRestoreViewModel)
        {
            _navigationService = navigationService;
            _signInViewModel = signInViewModel;
            _signUpViewModel = signUpViewModel;

            NavigateToSignInPage();

            _accountRestoreViewModel = accountRestoreViewModel;

            ISessionManager.UserLoggedIn += UserSingedIn;

            ISessionManager.UserLoggedOut += UserLoggedOut;
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

        private void UserSingedIn()
        {
            Authenticated = true;
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


        async partial void OnAuthenticatedChanged(bool value)
        {
            await NavigateToMenu();
        }


        [RelayCommand]
        public async Task NavigateToMenu()
        {
            if (Authenticated && CurrentViewModel.Role == Domain.Models.Enums.Roles.User)
                await _navigationService.NavigateBackAsync();
            else
                await _navigationService.NavigateAsync<MenuViewModel>();
        }
    }
}