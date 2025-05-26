namespace Restorator.Domain.Models.Restaurant
{
    public class CreateRestaurantDTO
    {
        public string Name { get; set; }
        public int TemplateId { get; set; }
        public string Description { get; set; }
        public TimeOnly BeginWorkTime { get; set; }
        public TimeOnly EndWorkTime { get; set; }

        public IEnumerable<byte[]> Images { get; set; }
        public byte[]? Menu { get; set; }
        public IEnumerable<int> Tags { get; set; }
    }
}
