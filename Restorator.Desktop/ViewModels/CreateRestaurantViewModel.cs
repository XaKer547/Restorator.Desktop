using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restorator.Domain.Models.Restaurant;
using Restorator.Domain.Models.Templates;
using Restorator.Domain.Services;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace Restorator.Desktop.ViewModels
{
    public partial class CreateRestaurantViewModel : RestaurantEditorViewModelBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly ISnackbarService _snackbarService;
        public CreateRestaurantViewModel(IRestaurantService restaurantService,
                                         Services.INavigationService navigationService,
                                         ISessionManager sessionManager,
                                         IContentDialogService contentDialogService,
                                         ISnackbarService snackbarService) : base(restaurantService, navigationService, sessionManager, contentDialogService)
        {
            _restaurantService = restaurantService;
            _snackbarService = snackbarService;
        }

        [ObservableProperty]
        private ObservableCollection<RestaurantTemplateDTO> templates = [];

        [ObservableProperty]
        private RestaurantTemplateDTO selectedTemplate;

        [RelayCommand]
        public async Task CreateRestaurant()
        {
            ValidateAllProperties();

            if (HasErrors)
            {
                var error = GetErrors().First();

                _snackbarService.Show("Так не пойдет", error.ErrorMessage!, Wpf.Ui.Controls.ControlAppearance.Danger);

                return;
            }

            var result = await _restaurantService.CreateRestaurant(new CreateRestaurantDTO
            {
                Name = RestaurantName,
                BeginWorkTime = TimeOnly.FromDateTime(BeginWorkTime),
                EndWorkTime = TimeOnly.FromDateTime(EndWorkTime),
                Description = Description,
                Images = await GetImagesBytes(),
                Menu = await GetMenuBytes(),
                Tags = RestaurantTags.Select(r => r.Id),
                TemplateId = SelectedTemplate.Id,
            });
        }

        [RelayCommand]
        public async Task LoadRestaurantTemplates()
        {
            Templates.Clear();

            foreach (var template in await _restaurantService.GetRestaurantTemplates())
                Templates.Add(template);
        }

        [RelayCommand]
        public async Task OpenExtendedRestaurantTemplate(RestaurantTemplateDTO restaurantTemplate)
        {
            //dialog
        }
    }
}