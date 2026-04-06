using Lumiere.Application.DTOs.Request.User;
using Lumiere.Application.DTOs.Response;
using Lumiere.Domain.Entities;

namespace Lumiere.Application.Interfaces.Repositoies;

public interface IUserRepository  : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetWithRoleAsync(int id, CancellationToken ct = default);
    Task<PagedResult<User>> GetFilteredAsync(UserQueryParams queryParams, CancellationToken ct = default);
}
