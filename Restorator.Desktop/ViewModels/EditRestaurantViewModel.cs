using CommunityToolkit.Mvvm.Input;
using Restorator.Desktop.Dialogs;
using Restorator.Desktop.Models;
using Restorator.Domain.Models.Restaurant;
using Restorator.Domain.Services;
using Wpf.Ui;
using Wpf.Ui.Extensions;

namespace Restorator.Desktop.ViewModels
{
    public partial class EditRestaurantViewModel : RestaurantEditorViewModelBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IContentDialogService _contentDialogService;
        private readonly Services.INavigationService _navigationService;
        private readonly ISnackbarService _snackbarService;
        public EditRestaurantViewModel(IRestaurantService restaurantService,
                                       Services.INavigationService navigationService,
                                       ISessionManager sessionManager,
                                       IContentDialogService contentDialogService,
                                       ISnackbarService snackbarService) : base(restaurantService, navigationService, sessionManager, contentDialogService)
        {
            _restaurantService = restaurantService;
            _contentDialogService = contentDialogService;
            _navigationService = navigationService;
            _snackbarService = snackbarService;
        }

        private int _restaurantId;
        public async Task LoadRestaurantInfo(int restaurantId)
        {
            _restaurantId = restaurantId;

            var result = await _restaurantService.GetRestaurantInfo(_restaurantId);

            if (result.IsFailed)
            {
                _snackbarService.Show("Ошибка", "Что-то пошло не так", Wpf.Ui.Controls.ControlAppearance.Danger);

                await _navigationService.NavigateBackAsync();

                return;
            }

            var info = result.Value;

            RestaurantName = info.Name;

            Description = info.Description;

            Approved = info.Approved;

            foreach (var image in info.Images)
                Images.Add(new RestaurantImageDTO() { Source = image });

            SelectedImage = Images.FirstOrDefault();

            if (info.Menu != null)
                Menu = new RestaurantImageDTO() { Source = info.Menu };

            RestaurantTags.Clear();

            foreach (var tag in info.Tags)
            {
                RestaurantTags.Add(tag);

                Tags.Add(tag);
            }

            BeginWorkTime = DateTime.Today.Add(info.BeginWorkTime.ToTimeSpan());

            EndWorkTime = DateTime.Today.Add(info.EndWorkTime.ToTimeSpan());
        }

        [RelayCommand]
        public async Task DeleteRestaurant()
        {
            var result = await _contentDialogService.ShowAsync(new ConfirmRestaurantDeletionContentDialog(), new CancellationToken());

            if (result != Wpf.Ui.Controls.ContentDialogResult.Primary)
                return;

            var v = await _restaurantService.DeleteRestaurant(_restaurantId);

            await _navigationService.NavigateBackAsync();
        }

        [RelayCommand]
        public async Task UpdateRestaurant()
        {
            ValidateAllProperties();

            if (HasErrors)
            {
                var error = GetErrors().First();

                _snackbarService.Show("Так не пойдет", error.ErrorMessage, Wpf.Ui.Controls.ControlAppearance.Danger);

                return;
            }

            var model = new UpdateRestraurantDTO
            {
                RestaurantId = _restaurantId,
                Name = RestaurantName,
                BeginWorkTime = TimeOnly.FromDateTime(BeginWorkTime),
                EndWorkTime = TimeOnly.FromDateTime(EndWorkTime),
                Description = Description,
                Images = await GetImagesBytes(),
                Menu = Menu is null ? null : await GetMenuBytes(),
                Tags = RestaurantTags.Select(r => r.Id)
            };

            var result = await _restaurantService.UpdateRestaurant(model);
        }
    }
}