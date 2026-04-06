using Lumiere.Application.DTOs.Request.Auth;
using Lumiere.Application.DTOs.Response;
using Lumiere.Application.DTOs.Response.Auth;

namespace Lumiere.Application.Interfaces.Services;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
}
 