namespace Lumiere.Application.Interfaces.Repositoies;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IArticleRepository Articles { get; }
    ICategoryRepository Categories { get; }
    IFavoriteRepository Favorites { get; }
    IRoleRepository Roles { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}