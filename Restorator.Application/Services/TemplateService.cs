using FluentResults;
using LinqKit;
using Restorator.Application.Client.Extensions;
using Restorator.Application.Client.Services.Abstract;
using Restorator.Domain.Models.Templates;
using Restorator.Domain.Services;

namespace Restorator.Application.Client.Services
{
    public class TemplateService : ApiClientBase, ITemplateService
    {
        public TemplateService(HttpClient client) : base(client, "template")
        { }

        public async Task<Result<int>> CreateRestaurantTemplate(CreateRestaurantTemplateDTO model)
        {
            var response = await PostAsJsonAsync("/table", model);

            return await response.AsResult<int>();
        }

        public async Task<Result<int>> CreateTableTemplate(CreateTableTempateDTO model)
        {
            var response = await PostAsJsonAsync("/restaurant", model);

            return await response.AsResult<int>();
        }

        public async Task<Result> DeleteRestaurantTemplate(int restaurantTemplateId)
        {
            var response = await DeleteAsync($"/restaurant/{restaurantTemplateId}");

            return await response.AsResult();
        }

        public async Task<IReadOnlyCollection<RestaurantTemplatePreview>> GetRestaurantsTemplatePreview()
        {
            var templatePreviews = await GetFromJsonAsync<IReadOnlyCollection<RestaurantTemplatePreview>>("/restaurant");

            templatePreviews.ForEach(x =>
            {
                x.Image = $"{_client.BaseAddress}/schemes/{x.Image}";
            });

            return templatePreviews ?? [];
        }

        public async Task<IReadOnlyCollection<TableTemplateDTO>> GetTableTemplates()
        {
            var templates = await GetFromJsonAsync<IReadOnlyCollection<TableTemplateDTO>>("/tables");

            return templates ?? [];
        }

        public async Task<Result> UpdateRestaurantTemplate(UpdateRestaurantTemplateDTO model)
        {
            var response = await PutAsJsonAsync("/restaurant", model);

            return await response.AsResult();
        }

        public async Task<Result<RestaurantTemplateDTO>> GetRestaurantTemplate(int restaurantTemplateId)
        {
            var template = await GetFromJsonAsync<RestaurantTemplateDTO>($"/restaurant/{restaurantTemplateId}");

            return template.ToResultWithNullCheck();
        }
    }
}