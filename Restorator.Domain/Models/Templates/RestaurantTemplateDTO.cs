namespace Restorator.Domain.Models.Templates
{
    public class RestaurantTemplateDTO
    {
        public int Id { get; set; }
        public IEnumerable<RestaurantTemplateTableDTO> Tables { get; set; }
        public string Scheme { get; set; }
    }
}
