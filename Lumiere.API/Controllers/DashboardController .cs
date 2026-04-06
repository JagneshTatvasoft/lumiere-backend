using Lumiere.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lumiere.API.Controllers;

[Authorize(Roles = "Admin")]
public class DashboardController : BaseApiController
{
    private readonly IDashboardService _dashboardService;
 
    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }
 
    /// <summary>Get aggregated dashboard statistics (Admin only).</summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken ct)
    {
        var result = await _dashboardService.GetStatsAsync(ct);
        return HandleResponse(result);
    }
}
