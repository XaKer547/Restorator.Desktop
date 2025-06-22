using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Domain.Models;
using Restorator.Domain.Models.Restaurant;
using Restorator.Domain.Services;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace Restorator.Desktop.ViewModels
{
    public partial class RestaurantSearchViewModel : ViewModelBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly Services.INavigationService _navigationService;
        public RestaurantSearchViewModel(IRestaurantService restaurantService,
                                         Services.INavigationService navigationService,
                                         ISessionManager sessionManager)
        {
            _restaurantService = restaurantService;
            _navigationService = navigationService;

            IsLoggedIn = sessionManager.HaveSession();

            sessionManager.UserLoggedIn += UserLoggedIn;
            sessionManager.UserLoggedOut += UserLoggedOut;
        }

        private void UserLoggedIn() => IsLoggedIn = true;
        private void UserLoggedOut() => IsLoggedIn = false;


        [ObservableProperty]
        private IReadOnlyCollection<RestaurantSearchItemDTO> _restaurantsNames = [];

        [ObservableProperty]
        private IReadOnlyCollection<RestaurantTagDTO> _restaurantsTag;

        [ObservableProperty]
        private ObservableCollection<RestaurantPreviewDTO> _restaurantsPreview = [];

        [ObservableProperty]
        private RestaurantTagDTO? _selectedTag;

        [ObservableProperty]
        private bool searching;

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private bool isLoggedIn;

        private CancellationTokenSource? _searchTokenSource = null;
        async partial void OnSearchTextChanging(string value)
        {
            if (value.Length < 1)
                return;

            if (value == string.Empty || string.IsNullOrWhiteSpace(value))
                RestaurantsNames = [];

            if (_searchTokenSource != null)
                await _searchTokenSource.CancelAsync();

            try
            {
                using (_searchTokenSource = _searchTokenSource ?? new CancellationTokenSource())
                    await Task.Delay(10, _searchTokenSource.Token).ContinueWith(async tr =>
                    {
                        if (!tr.IsCanceled)
                        {
                            RestaurantsNames = await _restaurantService.SearchRestaurants(value);
                        }
                    });
            }
            finally { _searchTokenSource = null; }
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task OpenRestaurantInfo(RestaurantPreviewDTO restaurantPreview)
        {
            await OpenRestaurantInfo(restaurantPreview!.Id);
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task OpenRestaurantInfoFromSuggestion(AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var item = (RestaurantSearchItemDTO)args.SelectedItem;

            await OpenRestaurantInfo(item.Id);
        }
        private async Task OpenRestaurantInfo(int id)
        {
            await _navigationService.NavigateWithHierarchyAsync<RestaurantInfoViewModel>(viewmodel => viewmodel.LoadRestaurantInfo(id));
        }

        private int _currentPage;
        private bool CanLoadRestaurants { get; set; }

        [RelayCommand]
        public async Task InitializeViewModel()
        {
            if (!Initialized)
            {
                SelectedTag = null;

                _currentPage = 1;

                RestaurantsTag = await _restaurantService.GetRestaurantsTags();

                Initialized = true;
            }

            Searching = true;

            RestaurantsPreview.Clear();

            await SearchRestaurants();

            Searching = false;
        }

        [ObservableProperty]
        private bool canResetTag;

        [RelayCommand]
        public async Task ChangeSearchTag(RestaurantTagDTO restaurantTag)
        {
            if (SelectedTag == restaurantTag)
                return;

            SelectedTag = restaurantTag;

            CanResetTag = true;
            IsEmptyLatest = false;

            await ResetSearch();
        }

        [RelayCommand]
        public async Task ResetSearch()
        {
            _currentPage = 1;

            RestaurantsPreview.Clear();

            Searching = true;

            await SearchRestaurants();

            Searching = false;
        }

        [RelayCommand]
        public async Task ResetSelectedTag()
        {
            if (SelectedTag == null && showedLatest)
                return;

            SelectedTag = null;
            IsEmptyLatest = false;
            CanResetTag = false;

            await ResetSearch();
        }

        [ObservableProperty]
        private bool isShowingLatest = false;

        private bool showedLatest = false;

        [ObservableProperty]
        private bool isEmptyLatest = false;

        partial void OnIsShowingLatestChanged(bool oldValue, bool newValue)
        {
            showedLatest = newValue;

            if (!showedLatest)
                IsEmptyLatest = false;
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        public async Task ShowLatest()
        {
            if (showedLatest)
                return;

            SelectedTag = null;

            RestaurantsPreview.Clear();

            Searching = true;

            var restaurants = await _restaurantService.GetLatestVisited();

            IsEmptyLatest = restaurants.Count == 0;

            foreach (var restaurant in restaurants)
                RestaurantsPreview.Add(restaurant);

            Searching = false;
        }


        [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanLoadRestaurants))]
        public async Task SearchRestaurants()
        {
            Searching = true;

            var restaurants = await _restaurantService.GetRestaurantPreviews(new GetRestaurantsPreviewDTO()
            {
                Filter = new GetRestaurantsPreviewFilter()
                {
                    RequireApproved = true,
                    TagId = SelectedTag?.Id,
                },
                PaginationFilter = new PaginationFilter()
                {
                    CurrentPage = _currentPage,
                    PageSize = 100 //Power check?
                }
            });

            //_currentPage++;

            CanLoadRestaurants = false;

            foreach (var restaurant in restaurants)
                RestaurantsPreview.Add(restaurant);

            Searching = false;
        }
    }
}