using Lumiere.Application.DTOs.Request.User;
using Lumiere.Application.DTOs.Response;
using Lumiere.Application.DTOs.Response.User;

namespace Lumiere.Application.Interfaces.Services;

public interface IUserService
{
    Task<ApiResponse<PagedResult<UserResponse>>> GetAllAsync(UserQueryParams queryParams, CancellationToken ct = default);
    
}
