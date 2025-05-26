using Restorator.Desktop.Models;
using Wpf.Ui.Controls;

namespace Restorator.Desktop.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для ReservationPage.xaml
    /// </summary>
    public partial class ConfirmReservationReservationContentDialog : ContentDialog
    {
        public ConfirmReservationReservationContentDialog(ConfirmRestaurantReservationModel model)
        {
            DataContext = model;
            InitializeComponent();
        }
    }
}
