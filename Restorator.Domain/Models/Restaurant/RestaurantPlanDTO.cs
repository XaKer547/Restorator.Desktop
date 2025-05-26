namespace Restorator.Domain.Models.Restaurant
{
    public class RestaurantPlanDTO
    {
        public int Id { get; set; }
        public string Scheme { get; set; }
        public TimeOnly BeginWorkTime { get; set; }
        public TimeOnly EndWorkTime { get; set; }
        public IReadOnlyCollection<TableDTO> Tables { get; set; }
    }
}
