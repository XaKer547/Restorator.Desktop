namespace Restorator.Domain.Models.Restaurant
{
    public class ChangeRestaurantApprovalDTO
    {
        public int RestaurantId { get; set; }
        public bool Approval { get; set; }
    }
}
