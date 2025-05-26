using CommunityToolkit.Mvvm.ComponentModel;

namespace Restorator.Desktop.Models
{
    public partial class ReservationModel : ObservableObject
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime ReservationStart { get; set; }
        public DateTime ReservationEnd { get; set; }

        [ObservableProperty]
        private bool canceled;

        partial void OnCanceledChanged(bool value)
        {
            OnPropertyChanged(nameof(CanCancel));
        }
        public bool CanCancel => ReservationEnd > DateTime.Now && !Canceled;
    }
}
