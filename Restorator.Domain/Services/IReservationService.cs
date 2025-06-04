using FluentResults;
using Restorator.Domain.Models.Reservations;
using Restorator.Domain.Models.Restaurant;

namespace Restorator.Domain.Services
{
    public interface IReservationService
    {
        Task<Result<RestaurantPlanDTO>> GetRestaurantReservationPlan(GetRestaurantPlanDTO model);
        Task<Result<int>> CreateReservation(CreateRestaurantReservationDTO model);
        Task<Result> CancelReservation(int reservationId);
        Task<Result<ReservationInfoDTO>> GetReservationInfo(int reservationId);
        Task<Result<IReadOnlyCollection<ReservationInfoDTO>>> GetReservations(GetReservationsDTO model);
        Task<Result<IReadOnlyCollection<ReservationInfoDTO>>> GetOwnedReservations(GetOwnedReservationsDTO model);
    }
}