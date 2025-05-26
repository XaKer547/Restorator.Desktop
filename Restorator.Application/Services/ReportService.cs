using Restorator.Application.Client.Services.Abstract;
using Restorator.Domain.Models.Reports;
using Restorator.Domain.Services;
using System.Text;

namespace Restorator.Application.Client.Services
{
    public class ReportService(HttpClient client) : ApiClientBase(client, "report"), IReportService
    {
        public Task<MonthSummaryReportDTO> GetMonthSummaryReport(DateOnly date, int? restaurantId = null)
        {
            var queryBuilder = new StringBuilder($"selectedDate={date:yyyy-MM-dd}");

            if (restaurantId.HasValue)
                queryBuilder.Append($"&restaurantId={restaurantId}");

            return GetFromJsonAsync<MonthSummaryReportDTO>($"?{queryBuilder}");
        }
    }
}
