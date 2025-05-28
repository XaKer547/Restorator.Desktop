using CommunityToolkit.Mvvm.ComponentModel;
using Restorator.Domain.Models.Enums;

namespace Restorator.Desktop.Models
{
    public partial class TableModel : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        private int? reservationId;

        [ObservableProperty]
        private TableStates state;

        [ObservableProperty]
        private double x;

        [ObservableProperty]
        private double y;

        [ObservableProperty]
        private double rotation;

        [ObservableProperty]
        private double height;

        [ObservableProperty]
        private double width;

        [ObservableProperty]
        private int templateId;
        private int TemplateModifierId => (int)(Rotation / 45);
        public int GetTemplateId() => TemplateId + TemplateModifierId;

        public TableModel Clone()
        {
            return new TableModel()
            {
                Height = Height,
                Width = Width,
                State = State,
                X = 0,
                Y = 0,
                Rotation = 0,
                TemplateId = TemplateId,
            };
        }
    }
}