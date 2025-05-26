namespace Restorator.Domain.Models.Reports
{
    public class RestaurantDailyReservationReportDTO
    {
        public string RestaurantName { get; set; }
        public IEnumerable<int> Reservations { get; set; }
    }
}