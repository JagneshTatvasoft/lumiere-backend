using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Domain.Entities;
using Lumiere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Lumiere.Infrastructure.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(LumiereJewelryDBContext context) : base(context) { }
 
    public async Task<Role?> GetByNameAsync(string name, CancellationToken ct = default)
        => await _context.Roles.FirstOrDefaultAsync(r => r.Name == name, ct);
}