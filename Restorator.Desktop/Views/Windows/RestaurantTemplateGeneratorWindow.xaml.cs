using Restorator.Desktop.Services;
using Restorator.Desktop.ViewModels;
using System.Windows;

namespace Restorator.Desktop.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для RestaurantTemplateGeneratorWindow.xaml
    /// </summary>
    public partial class RestaurantTemplateGeneratorWindow : Window, IDialog<RestaurantTemplateGeneratorViewModel>
    {
        public RestaurantTemplateGeneratorWindow(RestaurantTemplateGeneratorViewModel viewModel)
        {
            viewModel.DialogDoneEvent += ViewModel_DialogDoneEvent;

            DataContext = viewModel;

            InitializeComponent();
        }

        private void ViewModel_DialogDoneEvent(bool result)
        {
            DialogResult = result;
        }
    }
}
