using CommunityToolkit.Mvvm.ComponentModel;
using Restorator.Desktop.ViewModels;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Restorator.Desktop.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для RestaurantSearchPage.xaml
    /// </summary>
    [ObservableObject]
    public partial class RestaurantSearchPage : Page, INavigableView<RestaurantSearchViewModel>
    {
        public RestaurantSearchPage(RestaurantSearchViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;

            InitializeComponent();
        }

        public RestaurantSearchPage()
        {
            InitializeComponent();
        }

        public RestaurantSearchViewModel ViewModel { get; }
    }
}
