using System.Security.Claims;
using Lumiere.Application.DTOs.Response;
using Microsoft.AspNetCore.Mvc;

namespace Lumiere.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract  class BaseApiController : ControllerBase
{
    protected int CurrentUserId =>
        int.TryParse(User.FindFirstValue("userId"), out var id) ? id : 0;
 
    protected string CurrentUserRole =>
        User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
 
    protected IActionResult HandleResponse<T>(ApiResponse<T> response, int createdId = 0)
    {
        if (!response.Success)
        {
            return response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(response)
                : BadRequest(response);
        }
 
        if (createdId > 0)
            return CreatedAtAction(null, new { id = createdId }, response);
 
        return Ok(response);
    }
 
    protected IActionResult HandleResponse(ApiResponse response)
    {
        if (!response.Success)
        {
            return response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? NotFound(response)
                : BadRequest(response);
        }
        return Ok(response);
    }
}
