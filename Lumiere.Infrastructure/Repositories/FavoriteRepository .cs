using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Domain.Entities;
using Lumiere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumiere.Infrastructure.Repositories;

public class FavoriteRepository : Repository<FavoriteArticle>, IFavoriteRepository

{
    public FavoriteRepository(LumiereJewelryDBContext context) : base(context) { }
 
    public async Task<FavoriteArticle?> GetByUserAndArticleAsync(
        int userId, int articleId, CancellationToken ct = default)
        => await _context.FavoriteArticles
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ArticleId == articleId, ct);
 
    public async Task<List<FavoriteArticle>> GetByUserIdAsync(int userId, CancellationToken ct = default)
        => await _context.FavoriteArticles
            .Include(f => f.Article)
                .ThenInclude(a => a.Category)
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(ct);
}
