namespace Restorator.Domain.Models.Restaurant
{
    public class GetRestaurantsPreviewFilter
    {
        public int? TagId { get; set; }
        public bool? RequireApproved { get; set; }

        public bool IsDefault() => TagId.HasValue || RequireApproved.HasValue;
    }
}
