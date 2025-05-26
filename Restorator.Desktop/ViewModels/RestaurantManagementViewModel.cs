using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restorator.Desktop.Services;
using Restorator.Desktop.Session;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Domain.Models.Restaurant;
using Restorator.Domain.Services;
using System.Collections.ObjectModel;

namespace Restorator.Desktop.ViewModels
{
    public partial class RestaurantManagementViewModel : ViewModelBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly INavigationService _navigationService;
        public RestaurantManagementViewModel(IRestaurantService restaurantService,
                                             ISessionManager sessionManager,
                                             INavigationService navigationService)
        {
            _restaurantService = restaurantService;
            _navigationService = navigationService;
        }

        [ObservableProperty]
        private ObservableCollection<RestaurantPreviewDTO> restaurantsPreview = [];

        [ObservableProperty]
        private bool searching;

        [RelayCommand]
        public async Task LoadOwnedRestaurantsPreview()
        {
            Searching = true;

            RestaurantsPreview.Clear();

            var previews = await _restaurantService.GetOwnedRestaurantPreviews();

            foreach (var preview in previews)
                RestaurantsPreview.Add(preview);

            Searching = false;
        }

        [RelayCommand]
        public async Task OpenRestaurantReservations(RestaurantPreviewDTO restaurantPreview)
        {
            await _navigationService.NavigateWithHierarchyAsync<RestaurantReservationsManagementViewModel>(viewmodel => viewmodel.LoadRestaurantReservations(restaurantPreview.Id));
        }

        [RelayCommand]
        public async Task OpenRestaurantEditor(RestaurantPreviewDTO restaurantPreview)
        {
            await _navigationService.NavigateWithHierarchyAsync<EditRestaurantViewModel>(viewmodel => viewmodel.LoadRestaurantInfo(restaurantPreview.Id));
        }

        [RelayCommand]
        public async Task OpenRestaurantMaker()
        {
            await _navigationService.NavigateWithHierarchyAsync<CreateRestaurantViewModel>();
        }
    }
}
