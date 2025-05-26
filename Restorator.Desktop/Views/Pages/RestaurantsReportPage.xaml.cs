using Restorator.Desktop.ViewModels;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace Restorator.Desktop.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для RestraurantsReportPage.xaml
    /// </summary>
    public partial class RestraurantsReportPage : Page, INavigableView<RestraurantsReportViewModel>
    {
        public RestraurantsReportPage(RestraurantsReportViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = ViewModel;
            
            InitializeComponent();
        }

        public RestraurantsReportViewModel ViewModel { get; }
    }
}
