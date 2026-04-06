using Lumiere.Domain.Entities;

namespace Lumiere.Application.Interfaces.Repositoies;

public interface IRoleRepository : IRepository<Role>
{
     Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);
}

