using FluentResults;
using Restorator.Domain.Models.Templates;

namespace Restorator.Domain.Services
{
    public interface ITemplateService
    {
        Task<Result<int>> CreateTableTemplate(CreateTableTempateDTO model);
        Task<Result<int>> CreateRestaurantTemplate(CreateRestaurantTemplateDTO model);
        Task<IReadOnlyCollection<RestaurantTemplatePreview>> GetRestaurantsTemplatePreview();
        Task<Result<RestaurantTemplateDTO>> GetRestaurantTemplate(int restaurantTemplateId);
        Task<Result> DeleteRestaurantTemplate(int restaurantTemplateId);
        Task<IReadOnlyCollection<TableTemplateDTO>> GetTableTemplates();
        Task<Result> UpdateRestaurantTemplate(UpdateRestaurantTemplateDTO model);
    }
}