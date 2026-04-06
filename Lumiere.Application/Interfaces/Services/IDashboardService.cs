using Lumiere.Application.DTOs.Response;
using Lumiere.Application.DTOs.Response.Dashboard;

namespace Lumiere.Application.Interfaces.Services;

public interface IDashboardService
{
     Task<ApiResponse<DashboardStatsResponse>> GetStatsAsync(CancellationToken ct = default);
}
