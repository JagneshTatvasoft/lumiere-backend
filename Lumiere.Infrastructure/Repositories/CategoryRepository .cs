using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Domain.Entities;
using Lumiere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumiere.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(LumiereJewelryDBContext context) : base(context) { }

    public async Task<Category?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => await _context.Categories
            .FirstOrDefaultAsync(c => c.Slug == slug, ct);

    public async Task<Category?> GetWithArticlesAsync(int id, CancellationToken ct = default)
        => await _context.Categories
            .Include(c => c.Articles)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
}
