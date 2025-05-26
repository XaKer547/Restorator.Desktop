namespace Restorator.Domain.Models.Templates
{
    public class UpdateRestaurantTemplateDTO
    {
        public int Id { get; set; }
        public IEnumerable<UpdateRestaurantTemplateTableDTO> Tables { get; set; }
        public byte[] Scheme { get; set; }
    }
}
