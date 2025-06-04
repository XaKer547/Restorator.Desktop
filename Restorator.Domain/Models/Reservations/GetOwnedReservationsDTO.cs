namespace Restorator.Domain.Models.Reservations
{
    public class GetOwnedReservationsDTO
    {
        public DateOnly SelectedDate { get; set; }
        public int? RestaurantId { get; set; }
        public bool? SkipCanceled { get; set; }
    }
}
