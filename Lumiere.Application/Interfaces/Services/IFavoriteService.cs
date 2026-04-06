using Lumiere.Application.DTOs.Request.Favorite;
using Lumiere.Application.DTOs.Response;
using Lumiere.Application.DTOs.Response.Favorite;

namespace Lumiere.Application.Interfaces.Services;

public interface IFavoriteService
{
    Task<ApiResponse<ToggleFavoriteResponse>> ToggleAsync(ToggleFavoriteRequest request, CancellationToken ct = default);
    Task<ApiResponse<List<FavoriteResponse>>> GetByUserIdAsync(int userId, CancellationToken ct = default);
}
