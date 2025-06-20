using FluentResults;
using LinqKit;
using Restorator.Application.Client.Extensions;
using Restorator.Application.Client.Services.Abstract;
using Restorator.Domain.Models;
using Restorator.Domain.Models.Restaurant;
using Restorator.Domain.Models.Templates;
using Restorator.Domain.Services;
using System.Text;

namespace Restorator.Application.Client.Services
{
    public class RestaurantService(HttpClient client) : ApiClientBase(client, "restaurant"), IRestaurantService
    {
        public async Task<Result> ChangeRestaurantApproval(ChangeRestaurantApprovalDTO model)
        {
            var response = await PatchAsJsonAsync($"/{model.RestaurantId}/approve", model.Approval);

            return await response.AsResult();
        }

        public async Task<Result<int>> CreateRestaurant(CreateRestaurantDTO model)
        {
            var response = await PostAsJsonAsync(string.Empty, model);

            return await response.AsResult<int>();
        }

        public async Task<Result> DeleteRestaurant(int restaurantId)
        {
            var response = await DeleteAsync($"/{restaurantId}");

            return await response.AsResult();
        }

        public async Task<IReadOnlyCollection<RestaurantPreviewDTO>> GetOwnedRestaurantPreviews()
        {
            var ownedRestaurants = await GetFromJsonAsync<IReadOnlyCollection<RestaurantPreviewDTO>>("/owned");

            ownedRestaurants.ForEach(x =>
            {
                if (x.Image != null)
                    x.Image = $"{_client.BaseAddress}/restaurants/{x.Name}/{x.Image}";
            });

            return ownedRestaurants ?? [];
        }

        public async Task<Result<RestaurantInfoDTO>> GetRestaurantInfo(int restaurantId)
        {
            var info = await GetFromJsonAsync<RestaurantInfoDTO>($"/{restaurantId}");

            if (info.Menu != null)
                info.Menu = $"{_client.BaseAddress}/restaurants/{info.Name}/menu.png";

            info.Images = info.Images.Select(x => $"{_client.BaseAddress}/restaurants/{info.Name}/{x}");

            return info.ToResultWithNullCheck(); //useless :)
        }

        public async Task<IReadOnlyCollection<RestaurantSearchItemDTO>> SearchRestaurants(string? name, CancellationToken cancellationToken = default)
        {
            var names = await GetFromJsonAsync<IReadOnlyCollection<RestaurantSearchItemDTO>>($"/search?name={name}", cancellationToken);

            return names ?? [];
        }

        public async Task<PaginatedList<RestaurantPreviewDTO>> GetRestaurantPreviews(GetRestaurantsPreviewDTO model)
        {
            var pagintaion = model.PaginationFilter;

            var builder = new StringBuilder($"?PageSize={pagintaion.PageSize}&CurrentPage={pagintaion.CurrentPage}");

            var filter = model.Filter;

            if (filter is not null)
            {
                if (filter.TagId.HasValue)
                    builder.Append($"&tagId={filter.TagId}");

                if (filter.RequireApproved.HasValue)
                    builder.Append($"&requireApproved={filter.RequireApproved}");
            }

            var previews = await GetFromJsonAsync<PaginatedList<RestaurantPreviewDTO>>(builder.ToString());

            var extendedPreviews = previews.Select(x =>
            {
                string? image = null;

                if (x.Image != null)
                    image = $"{_client.BaseAddress}/restaurants/{x.Name}/{x.Image}";

                return new RestaurantPreviewDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Image = image,
                };
            });

            previews = new PaginatedList<RestaurantPreviewDTO>(previews.PageIndex, previews.Count, previews.ItemsPerPage, extendedPreviews);

            return previews ?? PaginatedList<RestaurantPreviewDTO>.Empty();
        }

        public async Task<IReadOnlyCollection<RestaurantTagDTO>> GetRestaurantsTags()
        {
            var tags = await GetFromJsonAsync<IReadOnlyCollection<RestaurantTagDTO>>("/tags");

            return tags ?? [];
        }

        public async Task<IReadOnlyCollection<RestaurantTemplateDTO>> GetRestaurantTemplates()
        {
            var templates = await GetFromJsonAsync<IReadOnlyCollection<RestaurantTemplateDTO>>("/templates");

            templates.ForEach(x =>
            {
                x.Scheme = $"{_client.BaseAddress}/schemes/{x.Scheme}";
            });

            return templates ?? [];
        }

        public async Task<Result> UpdateRestaurant(UpdateRestraurantDTO model)
        {
            var response = await PutAsJsonAsync(string.Empty, model);

            return await response.AsResult();
        }

        public async Task<IReadOnlyCollection<RestaurantPreviewDTO>> GetLatestVisited()
        {
            var latest = await GetFromJsonAsync<IReadOnlyCollection<RestaurantPreviewDTO>>("/latest");

            latest.ForEach(x =>
            {
                if (x.Image != null)
                    x.Image = $"{_client.BaseAddress}/restaurants/{x.Name}/{x.Image}";
            });

            return latest ?? [];
        }

        public async Task<IReadOnlyCollection<RestaurantSearchItemDTO>> GetOwnedRestaurantsSearchItems()
        {
            var ownedSearchItems = await GetFromJsonAsync<IReadOnlyCollection<RestaurantSearchItemDTO>>("/owned/search");

            return ownedSearchItems ?? [];
        }

        public Task<byte[]> GetImage(string uri)
        {
            return _client.GetByteArrayAsync(uri);
        }
    }
}