using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Infrastructure.Data;

namespace Lumiere.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{

    private readonly LumiereJewelryDBContext _context;

    private IUserRepository? _users;
    private IArticleRepository? _articles;
    private ICategoryRepository? _categories;
    private IFavoriteRepository? _favorites;
    private IRoleRepository? _roles;

    public UnitOfWork(LumiereJewelryDBContext context)
    {
        _context = context;
    }

    public IUserRepository Users
        => _users ??= new UserRepository(_context);

    public IArticleRepository Articles
        => _articles ??= new ArticleRepository(_context);

    public ICategoryRepository Categories
        => _categories ??= new CategoryRepository(_context);

    public IFavoriteRepository Favorites
        => _favorites ??= new FavoriteRepository(_context);

    public IRoleRepository Roles
        => _roles ??= new RoleRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
