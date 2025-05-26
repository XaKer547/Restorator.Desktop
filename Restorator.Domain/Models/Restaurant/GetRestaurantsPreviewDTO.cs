namespace Restorator.Domain.Models.Restaurant
{
    public class GetRestaurantsPreviewDTO
    {
        public PaginationFilter PaginationFilter { get; set; } = new PaginationFilter()
        {
            CurrentPage = 1,
            PageSize = 10
        };

        public GetRestaurantsPreviewFilter? Filter { get; set; }
    }
}
