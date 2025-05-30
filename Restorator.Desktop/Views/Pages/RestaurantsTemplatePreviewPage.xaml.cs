using Restorator.Desktop.ViewModels;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace Restorator.Desktop.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для RestaurantsTemplatePreviewPage.xaml
    /// </summary>
    public partial class RestaurantsTemplatePreviewPage : Page, INavigableView<RestaurantsTemplatePreviewViewModel>
    {
        public RestaurantsTemplatePreviewPage(RestaurantsTemplatePreviewViewModel viewmodel)
        {
            ViewModel = viewmodel;
            DataContext = ViewModel;

            InitializeComponent();
        }

        public RestaurantsTemplatePreviewViewModel ViewModel { get; }
    }
}
