using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Restorator.Desktop.Services;
using Restorator.Desktop.ViewModels.Abstract;
using Restorator.Domain.Models.Templates;
using Restorator.Domain.Services;
using System.Collections.ObjectModel;

namespace Restorator.Desktop.ViewModels
{
    public partial class RestaurantsTemplatePreviewViewModel : ViewModelBase
    {
        private readonly IWindowManager _windowManager;
        private readonly ITemplateService _templateService;
        public RestaurantsTemplatePreviewViewModel(IWindowManager windowManager, ITemplateService templateService)
        {
            _windowManager = windowManager;
            _templateService = templateService;
        }

        [ObservableProperty]
        private ObservableCollection<RestaurantTemplatePreview> templates = [];

        [RelayCommand]
        private async Task Initialize()
        {
            await LoadTemplates();

            Initialized = true;
        }

        [RelayCommand]
        private async Task OpenEditor()
        {
            var success = _windowManager.ShowWindow<RestaurantTemplateGeneratorViewModel>();

            if (success)
                await LoadTemplates();
        }

        private async Task LoadTemplates()
        {
            Templates.Clear();

            foreach (var template in await _templateService.GetRestaurantsTemplatePreview())
                Templates.Add(template);
        }
    }
}