using Restorator.Domain.Models.Reports;

namespace Restorator.Domain.Services
{
    public interface IReportService
    {
        Task<MonthSummaryReportDTO> GetMonthSummaryReport(DateOnly date, int? restaurantId = default);
    }
}
