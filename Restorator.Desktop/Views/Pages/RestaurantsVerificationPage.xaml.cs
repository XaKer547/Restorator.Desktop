using Restorator.Desktop.ViewModels;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace Restorator.Desktop.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для RestaurantsVerificationPage.xaml
    /// </summary>
    public partial class RestaurantsVerificationPage : Page, INavigableView<RestaurantsVerificationViewModel>
    {
        public RestaurantsVerificationPage(RestaurantsVerificationViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;

            InitializeComponent();
        }

        public RestaurantsVerificationViewModel ViewModel { get; }
    }
}
