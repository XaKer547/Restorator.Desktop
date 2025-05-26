using FluentResults;
using Restorator.Application.Client.Extensions;
using Restorator.Application.Client.Services.Abstract;
using Restorator.Domain.Models.Reservations;
using Restorator.Domain.Models.Restaurant;
using Restorator.Domain.Services;
using System.Text;

namespace Restorator.Application.Client.Services
{
    public class ReservationService : ApiClientBase, IReservationService
    {
        public ReservationService(HttpClient client) : base(client, "reservation") { }

        public async Task<Result> CancelReservation(int reservationId)
        {
            var response = await HeadAsync($"/{reservationId}/cancel");

            return await response.AsResult();
        }

        public async Task<Result<int>> CreateReservation(CreateRestaurantReservationDTO model)
        {
            var response = await PostAsJsonAsync(string.Empty, model);

            return await response.AsResult<int>();
        }

        public async Task<Result<ReservationInfoDTO>> GetReservationInfo(int reservationId)
        {
            var plan = await GetFromJsonAsync<ReservationInfoDTO>($"/{reservationId}");

            return plan.ToResultWithNullCheck();
        }

        public async Task<Result<IReadOnlyCollection<ReservationInfoDTO>>> GetReservations(GetReservationsDTO model)
        {
            var builder = new StringBuilder($"?selectedDate={model.SelectedDate:yyyy-MM-dd}");

            if (model.RestaurantId.HasValue)
                builder.Append($"&restaurantId={model.RestaurantId}");

            if (model.UserId.HasValue)
                builder.Append($"&userId={model.UserId}");

            if (model.SkipCanceled.HasValue)
                builder.Append($"&skipCanceled={model.SkipCanceled}");

            var reservations = await GetFromJsonAsync<IReadOnlyCollection<ReservationInfoDTO>>(builder.ToString());

            return reservations.ToResultWithNullCheck();
        }

        public async Task<Result<RestaurantPlanDTO>> GetRestaurantReservationPlan(GetRestaurantPlanDTO model)
        {
            var plan = await GetFromJsonAsync<RestaurantPlanDTO>($"/{model.RestaurantId}/plan?ReservationStartDate={model.ReservationStartDate}&ReservationEndDate={model.ReservationEndDate}");

            return plan.ToResultWithNullCheck();
        }
    }
}
