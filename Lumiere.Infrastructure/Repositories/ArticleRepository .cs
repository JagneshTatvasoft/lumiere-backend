using Lumiere.Application.DTOs.Request.Article;
using Lumiere.Application.DTOs.Response;
using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Domain.Entities;
using Lumiere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumiere.Infrastructure.Repositories;

public class ArticleRepository : Repository<Article>, IArticleRepository
{
     public ArticleRepository(LumiereJewelryDBContext context) : base(context) { }
 
    public async Task<Article?> GetWithCategoryAsync(int id, CancellationToken ct = default)
        => await _context.Articles
            .Include(a => a.Category)
            .Include(a => a.FavoriteArticles)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
 
    public async Task<PagedResult<Article>> GetFilteredAsync(ArticleQueryParams q, CancellationToken ct = default)
    {
        var query = _context.Articles
            .Include(a => a.Category)
            .Include(a => a.FavoriteArticles)
            .AsQueryable();
 
        if (!string.IsNullOrWhiteSpace(q.Search))
            query = query.Where(a =>
                a.Name.Contains(q.Search) ||
                a.ShortDescription.Contains(q.Search));
 
        if (q.CategoryId.HasValue)
            query = query.Where(a => a.CategoryId == q.CategoryId.Value);
 
        if (q.InStock.HasValue)
            query = query.Where(a => a.Stock >= 0);
 
        // Sorting
        query = (q.SortBy?.ToLower(), q.SortDir?.ToLower()) switch
        {
            ("price", "asc")  => query.OrderBy(a => a.Price),
            ("price", _)      => query.OrderByDescending(a => a.Price),
            ("name",  "asc")  => query.OrderBy(a => a.Name),
            ("name",  _)      => query.OrderByDescending(a => a.Name),
            _                 => query.OrderByDescending(a => a.CreatedAt)
        };
 
        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .ToListAsync(ct);
 
        return new PagedResult<Article>
        {
            Items = items,
            TotalCount = total,
            Page = q.Page,
            PageSize = q.PageSize
        };
    }
}
