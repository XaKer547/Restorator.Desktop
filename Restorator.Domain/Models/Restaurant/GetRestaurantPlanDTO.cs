namespace Restorator.Domain.Models.Restaurant
{
    public class GetRestaurantPlanDTO
    {
        public int RestaurantId { get; set; }
        public DateTime ReservationStartDate { get; set; }
        public DateTime ReservationEndDate { get; set; }
    }
}
