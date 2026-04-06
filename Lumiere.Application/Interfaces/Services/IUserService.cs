using Lumiere.Application.DTOs.Request.User;
using Lumiere.Application.DTOs.Response;
using Lumiere.Application.DTOs.Response.User;

namespace Lumiere.Application.Interfaces.Services;

public interface IUserService
{
    Task<ApiResponse<PagedResult<UserResponse>>> GetAllAsync(UserQueryParams queryParams, CancellationToken ct = default);
    Task<ApiResponse<UserResponse>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ApiResponse<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<ApiResponse<UserResponse>> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default);
    Task<ApiResponse> DeleteAsync(int id, CancellationToken ct = default);
}
