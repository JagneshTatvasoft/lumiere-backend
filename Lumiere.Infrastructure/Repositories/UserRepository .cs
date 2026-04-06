using Lumiere.Application.DTOs.Request.User;
using Lumiere.Application.DTOs.Response;
using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Domain.Entities;
using Lumiere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumiere.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(LumiereJewelryDBContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetWithRoleAsync(int id, CancellationToken ct = default)
        => await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<PagedResult<User>> GetFilteredAsync(UserQueryParams q, CancellationToken ct = default)
    {
        var query = _context.Users
            .Include(u => u.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q.Search))
            query = query.Where(u =>
                u.Name.Contains(q.Search) ||
                u.Email.Contains(q.Search));

        if (q.RoleId.HasValue)
            query = query.Where(u => u.RoleId == q.RoleId.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .ToListAsync(ct);

        return new PagedResult<User>
        {
            Items = items,
            TotalCount = total,
            Page = q.Page,
            PageSize = q.PageSize
        };
    }

}
