using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Restorator.Desktop.Models;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Domain.Models.Restaurant;
using Restorator.Domain.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Wpf.Ui;

namespace Restorator.Desktop.ViewModels
{
    public partial class RestaurantEditorViewModelBase : ViewModelBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly Services.INavigationService _navigationService;
        private readonly IContentDialogService _contentDialogService;
        public RestaurantEditorViewModelBase(IRestaurantService restaurantService,
                                         Services.INavigationService navigationService,
                                         ISessionManager sessionManager,
                                         IContentDialogService contentDialogService)
        {
            _restaurantService = restaurantService;
            _navigationService = navigationService;

            _contentDialogService = contentDialogService;
        }

        protected readonly int _userId;

        [ObservableProperty]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Название ресторана обязательно для заполнения")]
        private string restaurantName;

        [ObservableProperty]
        [Required(AllowEmptyStrings = false, ErrorMessage = "У ресторана должно быть описание")]
        private string description;

        [ObservableProperty]
        private bool approved;

        [ObservableProperty]
        private RestaurantImageDTO? selectedImage;

        [ObservableProperty]
        private ObservableCollection<RestaurantImageDTO> images = [];

        [ObservableProperty]
        private RestaurantImageDTO? menu;

        [ObservableProperty]
        private DateTime beginWorkTime;

        [ObservableProperty]
        private DateTime endWorkTime;

        [ObservableProperty]
        [MinLength(1, ErrorMessage = "Должен быть выбран хотя-бы один тэг")]
        private ObservableCollection<RestaurantTagDTO> restaurantTags = [];

        [ObservableProperty]
        private ObservableCollection<RestaurantTagDTO> tags = [];


        [RelayCommand]
        public void SetSelectedImage(RestaurantImageDTO image)
        {
            SelectedImage = image;
        }

        [RelayCommand]
        public async Task LoadRestaurantTags()
        {
            foreach (var tag in await _restaurantService.GetRestaurantsTags())
                if (!Tags.Any(t => t.Id == tag.Id))
                    Tags.Add(tag);
        }

        [RelayCommand]
        public void AddRestaurantImage()
        {
            if (!TryLoadImage(out var image))
            {
                return;
            }

            Images.Add(image);

            SelectedImage ??= image;
        }


        [RelayCommand]
        public void DeleteRestaurantImage(RestaurantImageDTO image)
        {
            Images.Remove(image);

            if (SelectedImage == image)
                SelectedImage = Images.FirstOrDefault();
        }


        [RelayCommand]
        public void ReplaceRestaurantImage(RestaurantImageDTO image)
        {
            if (!TryLoadImage(out var newImage))
            {
                return;
            }

            image.Source = newImage.Source;
            image.IsLocal = true;
        }

        [RelayCommand]
        public void LoadRestaurantMenuImage()
        {
            if (!TryLoadImage(out var newMenu))
                return;

            Menu = newMenu;
        }

        [RelayCommand]
        public void DeleteRestaurantMenuImage()
        {
            Menu = null;
        }

        [RelayCommand]
        public async Task CloseRestaurantEditor() => await _navigationService.NavigateBackAsync();

        private bool TryLoadImage(out RestaurantImageDTO image)
        {
            image = null;

            var dialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = "Изображения|*.jpg;*.jpeg;*.png;"
            };

            if (dialog.ShowDialog() != true)
                return false;

            image = new RestaurantImageDTO()
            {
                IsLocal = true,
                Source = dialog.FileName
            };

            return true;
        }

        private Task<byte[]> PrepareImage(RestaurantImageDTO image)
        {
            if (image.IsLocal)
                return File.ReadAllBytesAsync(image.Source);

            return _restaurantService.GetImage(image.Source);
        }

        protected async Task<IEnumerable<byte[]>?> GetImagesBytes()
        {
            if (Images.Count == 0)
                return null;

            var imagesList = new List<byte[]>();

            foreach (var image in Images)
            {
                var bytes = await PrepareImage(image);

                imagesList.Add(bytes);
            }

            return imagesList;
        }


        protected Task<byte[]> GetMenuBytes()
        {
            return PrepareImage(Menu!);
        }
    }
}