using AutoMapper;
using Lumiere.Application.DTOs.Request.User;
using Lumiere.Application.DTOs.Response;
using Lumiere.Application.DTOs.Response.User;
using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Application.Interfaces.Services;
using Lumiere.Domain.Entities;

namespace Lumiere.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
 
    public UserService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }
 
    public async Task<ApiResponse<PagedResult<UserResponse>>> GetAllAsync(UserQueryParams q, CancellationToken ct = default)
    {
        var paged = await _uow.Users.GetFilteredAsync(q, ct);
        var mapped = new PagedResult<UserResponse>
        {
            Items = _mapper.Map<List<UserResponse>>(paged.Items),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
        return ApiResponse<PagedResult<UserResponse>>.Ok(mapped);
    }
 
}