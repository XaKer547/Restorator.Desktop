using Restorator.Domain.Models.Enums;

namespace Restorator.Domain.Models.Restaurant
{
    public class TableDTO
    {
        public int Id { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }

        public float X { get; set; }
        public float Y { get; set; }

        public float Rotation { get; set; }

        public TableStates State { get; set; }
    }
}
