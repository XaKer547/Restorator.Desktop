namespace Restorator.Domain.Models.Templates
{
    public class CreateRestaurantTemplateDTO
    {
        public IEnumerable<CreateRestaurantTemplateTableDTO> Tables { get; set; }
        public byte[] Scheme { get; set; }
    }
}