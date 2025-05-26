using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restorator.Desktop.Services;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Domain.Models;
using Restorator.Domain.Models.Restaurant;
using Restorator.Domain.Services;
using System.Collections.ObjectModel;

namespace Restorator.Desktop.ViewModels
{
    public partial class RestaurantsVerificationViewModel : ViewModelBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly INavigationService _navigationService;
        public RestaurantsVerificationViewModel(IRestaurantService restaurantService,
                                                INavigationService navigationService)
        {
            _restaurantService = restaurantService;
            _navigationService = navigationService;
        }

        [ObservableProperty]
        private bool? showVerified = false;

        [ObservableProperty]
        private ObservableCollection<RestaurantPreviewDTO> previews = [];



        private int _currentPage;
        private bool CanLoadRestaurants { get; set; }

        [RelayCommand]
        public async Task InitializeRestaurantsPreview()
        {
            Previews.Clear();

            _currentPage = 1;

            await LoadRestaurantsPreview();
        }

        [RelayCommand(CanExecute = nameof(CanLoadRestaurants))]
        private async Task LoadRestaurantsPreview()
        {
            var previewsList = await _restaurantService.GetRestaurantPreviews(new GetRestaurantsPreviewDTO()
            {
                Filter = new GetRestaurantsPreviewFilter()
                {
                    RequireApproved = ShowVerified,
                },
                PaginationFilter = new PaginationFilter()
                {
                    PageSize = 20,
                    CurrentPage = _currentPage
                }
            });

            CanLoadRestaurants = previewsList.HasNextPage;

            _currentPage++;

            foreach (var preview in previewsList)
            {
                Previews.Add(preview);
            }
        }

        async partial void OnShowVerifiedChanged(bool? value)
        {
            await InitializeRestaurantsPreview();
        }


        [RelayCommand]
        public async Task OpenRestaurantVerification(RestaurantPreviewDTO restaurantPreview)
        {
            await _navigationService.NavigateWithHierarchyAsync<RestaurantVerificationViewModel>(viewmodel =>
            viewmodel.LoadRestaurantInfo(restaurantPreview.Id));
        }
    }
}
