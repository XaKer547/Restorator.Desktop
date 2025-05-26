using FluentResults;
using Restorator.Domain.Models;
using Restorator.Domain.Models.Restaurant;
using Restorator.Domain.Models.Templates;

namespace Restorator.Domain.Services
{
    public interface IRestaurantService
    {
        Task<Result<int>> CreateRestaurant(CreateRestaurantDTO model);
        Task<Result<RestaurantInfoDTO>> GetRestaurantInfo(int restaurantId);
        Task<IReadOnlyCollection<RestaurantSearchItemDTO>> SearchRestaurants(string? name, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<RestaurantTagDTO>> GetRestaurantsTags();
        Task<IReadOnlyCollection<RestaurantPreviewDTO>> GetOwnedRestaurantPreviews();
        Task<PaginatedList<RestaurantPreviewDTO>> GetRestaurantPreviews(GetRestaurantsPreviewDTO model);
        Task<Result> ChangeRestaurantApproval(ChangeRestaurantApprovalDTO model);
        Task<Result> DeleteRestaurant(int restaurantId);
        Task<Result> UpdateRestaurant(UpdateRestraurantDTO model);
        Task<IReadOnlyCollection<RestaurantTemplateDTO>> GetRestaurantTemplates();
        Task<IReadOnlyCollection<RestaurantPreviewDTO>> GetLatestVisited();
        Task<IReadOnlyCollection<RestaurantSearchItemDTO>> GetOwnedRestaurantsSearchItems();
    }
}
