using Restorator.Desktop.Models;
using Restorator.Domain.Models.Templates;

namespace Restorator.Desktop.Extensions
{
    public static class TableTemplateDTOExtensions
    {
        public static TableModel ToModel(this TableTemplateDTO template)
        {
            return new()
            {
                TemplateId = template.Id,
                Height = template.Height,
                Width = template.Width,
                State = Domain.Models.Enums.TableStates.OccupiedByUser,
            };
        }
    }
}
