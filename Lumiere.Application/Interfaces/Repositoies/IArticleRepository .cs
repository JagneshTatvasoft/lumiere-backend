using Lumiere.Application.DTOs.Request.Article;
using Lumiere.Application.DTOs.Response;
using Lumiere.Domain.Entities;

namespace Lumiere.Application.Interfaces.Repositoies;

public interface IArticleRepository : IRepository<Article>
{
     Task<Article?> GetWithCategoryAsync(int id, CancellationToken ct = default);
    Task<PagedResult<Article>> GetFilteredAsync(ArticleQueryParams queryParams, CancellationToken ct = default);
}
