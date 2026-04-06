using Lumiere.Application.DTOs.Request.Category;
using Lumiere.Application.DTOs.Response;
using Lumiere.Application.DTOs.Response.Category;

namespace Lumiere.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<ApiResponse<List<CategoryResponse>>> GetAllAsync(CancellationToken ct = default);
    Task<ApiResponse<CategoryResponse>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ApiResponse<CategoryResponse>> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default);
    Task<ApiResponse<CategoryResponse>> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken ct = default);
    Task<ApiResponse> DeleteAsync(int id, CancellationToken ct = default);
}
