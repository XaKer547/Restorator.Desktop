namespace Restorator.Domain.Models.Reports
{
    public class ReservationsRateReportDTO
    {
        public DateOnly Date { get; set; }
        public int Rate { get; set; }
        public string RestaurantName { get; set; }
    }
}
