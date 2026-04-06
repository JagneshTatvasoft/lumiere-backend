using FluentValidation;
using Lumiere.Application.DTOs.Request.Favorite;
using Lumiere.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lumiere.API.Controllers;

public class FavoritesController : BaseApiController
{
    private readonly IFavoriteService _favoriteService;
    private readonly IValidator<ToggleFavoriteRequest> _toggleValidator;
 
    public FavoritesController(
        IFavoriteService favoriteService,
        IValidator<ToggleFavoriteRequest> toggleValidator)
    {
        _favoriteService = favoriteService;
        _toggleValidator = toggleValidator;
    }
 
    /// <summary>Toggle a like or dislike reaction on an article.</summary>
    [HttpPost("toggle")]
    [Authorize]
    public async Task<IActionResult> Toggle([FromBody] ToggleFavoriteRequest request, CancellationToken ct)
    {
        // Enforce: users can only react on their own behalf
        if (CurrentUserRole != "Admin" && CurrentUserId != request.UserId)
            return Forbid();
 
        var validation = await _toggleValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return BadRequest(new { success = false, errors = validation.Errors.Select(e => e.ErrorMessage) });
 
        var result = await _favoriteService.ToggleAsync(request, ct);
        return HandleResponse(result);
    }
 
    /// <summary>Get all favorites (likes + dislikes) for a user.</summary>
    [HttpGet("user/{userId:int}")]
    [Authorize]
    public async Task<IActionResult> GetByUser(int userId, CancellationToken ct)
    {
        if (CurrentUserRole != "Admin" && CurrentUserId != userId)
            return Forbid();
 
        var result = await _favoriteService.GetByUserIdAsync(userId, ct);
        return HandleResponse(result);
    }
}
