using Lumiere.Domain.Entities;

namespace Lumiere.Application.Interfaces.Repositoies;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<Category?> GetWithArticlesAsync(int id, CancellationToken ct = default);
}