using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restorator.Desktop.Dialogs;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Desktop.Views.Pages;
using Restorator.Domain.Models.Enums;
using Restorator.Domain.Services;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Restorator.Desktop.ViewModels
{
    public partial class MenuViewModel : ViewModelBase
    {
        private readonly IPageService _pageService;
        private readonly Services.INavigationService _navigationService;
        private readonly ISessionManager _sessionManager;
        private readonly IContentDialogService _contentDialogService;
        private readonly INavigationService _menuNavigationService;

        public MenuViewModel(IPageService pageService,
                             Services.INavigationService navigationService,
                             ISessionManager sessionManager,
                             IContentDialogService contentDialogService,
                             INavigationService menuNavigationService)
        {
            _pageService = pageService;
            _navigationService = navigationService;
            _sessionManager = sessionManager;

            if (!_sessionManager.TryGetSession(out var sessionInfo))
            {
                Username = "Гость";
                _role = null;
            }
            else
            {
                _role = Enum.Parse<Roles>(sessionInfo.Role);
                Username = sessionInfo.Username;
            }

            _contentDialogService = contentDialogService;
            _menuNavigationService = menuNavigationService;

            ISessionManager.UserLoggedIn += RefreshMenuState;

            ISessionManager.UserLoggedOut += RefreshMenuState;
        }


        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private ObservableCollection<object> menuItems = [];

        [ObservableProperty]
        private ObservableCollection<object> footerItems = [];

        private Roles? _role;


        [RelayCommand]
        public void RefreshMenuState()
        {
            if (!_sessionManager.TryGetSession(out var sessionInfo))
            {
                Username = "Гость";
                _role = null;
            }
            else
            {
                Username = sessionInfo.Username;
                _role = Enum.Parse<Roles>(sessionInfo.Role);
            }

            MenuItems.Clear();
            FooterItems.Clear();

            InitializeNavigationItems();
        }

        [RelayCommand]
        public void ConfigurePageService(INavigationView navigationView)
        {
            navigationView.SetPageService(_pageService);

            _menuNavigationService.SetNavigationControl(navigationView);

            if (navigationView.MenuItems.Count == 0)
                InitializeNavigationItems();

            var item = (NavigationViewItem)MenuItems[0];

            _menuNavigationService.Navigate(item.TargetPageType!);
        }

        private void InitializeNavigationItems()
        {
            if (_role == Roles.User)
            {
                MenuItems.Add(new NavigationViewItem("Поиск", SymbolRegular.Search16, typeof(RestaurantSearchPage)));
                MenuItems.Add(new NavigationViewItem("Бронирования", SymbolRegular.BookOpen16, typeof(UserReservationsPage)));
            }

            if (_role == Roles.Manager)
            {
                MenuItems.Add(new NavigationViewItem("Управление", SymbolRegular.FolderPeople20, typeof(RestaurantManagementPage)));
                MenuItems.Add(new NavigationViewItem("Статистика", SymbolRegular.Diagram20, typeof(RestraurantsReportPage)));

            }

            if (_role == Roles.Admin)
            {
                MenuItems.Add(new NavigationViewItem("Заявки", SymbolRegular.TaskListRtl20, typeof(RestaurantsVerificationPage)));
                MenuItems.Add(new NavigationViewItem("Редактор схем", SymbolRegular.TaskListRtl20, typeof(RestaurantTemplateGeneratorPage)));
            }

            if (_role is null)
            {
                MenuItems.Add(new NavigationViewItem("Поиск", SymbolRegular.Search16, typeof(RestaurantSearchPage)));

                FooterItems.Add(new NavigationViewItem
                {
                    Icon = new SymbolIcon(SymbolRegular.DoorArrowRight16),
                    Content = "Войти",
                    Command = LoginCommand
                });
            }
            else
            {
                FooterItems.Add(new NavigationViewItem
                {
                    Icon = new SymbolIcon(SymbolRegular.DoorArrowLeft16),
                    Content = "Выйти",
                    Command = LogoutCommand
                });
            }
        }

        [RelayCommand]
        public async Task Login() => await _navigationService.NavigateWithHierarchyAsync<AuthenticationViewModel>();

        [RelayCommand]
        public async Task Logout()
        {
            var result = await _contentDialogService.ShowAsync(new ConfirmLogoutContentDialog(), new CancellationToken());

            if (result != ContentDialogResult.Primary)
                return;

            _sessionManager.RemoveSession();

            MenuItems.Clear();
            FooterItems.Clear();

            _role = null;
            Username = "Гость";

            InitializeNavigationItems();

            var item = (NavigationViewItem)MenuItems[0];

            _menuNavigationService.Navigate(item.TargetPageType!);
        }
    }
}