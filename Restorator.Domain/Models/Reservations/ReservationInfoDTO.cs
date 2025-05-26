namespace Restorator.Domain.Models.Reservations
{
    public class ReservationInfoDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public DateTime ReservationStart { get; set; }
        public DateTime ReservationEnd { get; set; }
        public bool Canceled { get; set; }
        public bool CanCancel => DateTime.Now < ReservationEnd && !Canceled;
    }
}