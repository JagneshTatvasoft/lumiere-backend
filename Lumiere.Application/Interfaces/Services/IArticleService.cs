using Lumiere.Application.DTOs.Request.Article;
using Lumiere.Application.DTOs.Response;
using Lumiere.Application.DTOs.Response.Article;

namespace Lumiere.Application.Interfaces.Services;

public interface IArticleService
{
    Task<ApiResponse<PagedResult<ArticleResponse>>> GetAllAsync(ArticleQueryParams queryParams, CancellationToken ct = default);
    Task<ApiResponse<ArticleResponse>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ApiResponse<ArticleResponse>> CreateAsync(CreateArticleRequest request, CancellationToken ct = default);
    Task<ApiResponse<ArticleResponse>> UpdateAsync(int id, UpdateArticleRequest request, CancellationToken ct = default);
    Task<ApiResponse> DeleteAsync(int id, CancellationToken ct = default);
}
