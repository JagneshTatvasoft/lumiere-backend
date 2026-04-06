using Lumiere.Domain.Entities;

namespace Lumiere.Application.Interfaces.Repositoies;

public interface IFavoriteRepository : IRepository<FavoriteArticle>
{
    Task<FavoriteArticle?> GetByUserAndArticleAsync(int userId, int articleId, CancellationToken ct = default);
    Task<List<FavoriteArticle>> GetByUserIdAsync(int userId, CancellationToken ct = default);
}
 