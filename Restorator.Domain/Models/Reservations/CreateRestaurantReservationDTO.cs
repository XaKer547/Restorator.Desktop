namespace Restorator.Domain.Models.Reservations
{
    public class CreateRestaurantReservationDTO
    {
        public int RestaurantId { get; set; }
        public IReadOnlyCollection<int> ReservedTables { get; set; }
        public DateTime ReservationStartDate { get; set; }
        public DateTime ReservationEndDate { get; set; }
    }
}