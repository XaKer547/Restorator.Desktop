namespace Restorator.Desktop.Models
{
    public class ConfirmRestaurantReservationModel
    {
        public TimeOnly ReservationStart { get; set; }
        public TimeOnly ReservationEnd { get; set; }
        public int TablesCount { get; set; }
    }
}
