namespace Restorator.Domain.Models.Reports
{
    public class RestaurantStatisticsDTO
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public int CanceledOrders { get; set; }
        public int FinishedOrders { get; set; }
        public int WaitingOrders { get; set; }
        public int TotalOrders => CanceledOrders + FinishedOrders + WaitingOrders;
    }
}
