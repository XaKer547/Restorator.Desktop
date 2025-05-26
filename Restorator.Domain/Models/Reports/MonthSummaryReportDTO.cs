namespace Restorator.Domain.Models.Reports
{
    public class MonthSummaryReportDTO
    {
        public bool IsEmpty { get; set; }
        public int Reserved { get; set; }
        public int Finished { get; set; }
        public int Canceled { get; set; }

        public IEnumerable<RestaurantDailyReservationReportDTO> RestaurantDailyReservations { get; set; }
        public string MostPopularDay { get; set; }
        public MonthPopularRestaurantReportDTO MostPopularRestaurant { get; set; }
        public ReservationsRateReportDTO ReservationsRate { get; set; }
    }
}