using CommunityToolkit.Mvvm.ComponentModel;
using Restorator.Desktop.ViewModels.Abstract;

namespace Restorator.Desktop.ViewModels
{
    public partial class TableReservationContentDialogViewModel : ViewModelBase
    {
        [ObservableProperty]
        private TimeOnly selectedTime;
    }
}
