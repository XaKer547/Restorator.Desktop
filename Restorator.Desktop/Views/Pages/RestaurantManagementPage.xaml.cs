using Restorator.Desktop.ViewModels;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace Restorator.Desktop.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для RestaurantManagementPage.xaml
    /// </summary>
    public partial class RestaurantManagementPage : Page, INavigableView<RestaurantManagementViewModel>
    {
        public RestaurantManagementPage(RestaurantManagementViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;

            InitializeComponent();
        }

        public RestaurantManagementViewModel ViewModel { get; }
    }
}
