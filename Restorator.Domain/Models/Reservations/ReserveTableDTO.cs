namespace Restorator.Domain.Models.Reservations
{
    public class ReserveTableDTO
    {
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public int TableId { get; set; }
        public DateTime ReservationStart { get; set; }
        public DateTime ReservationEnd { get; set; }
    }
}