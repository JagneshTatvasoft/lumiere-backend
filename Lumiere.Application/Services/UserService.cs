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
 
    public async Task<ApiResponse<UserResponse>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetWithRoleAsync(id, ct);
        if (user == null || user.IsDeleted)
            return ApiResponse<UserResponse>.Fail("User not found.");
 
        return ApiResponse<UserResponse>.Ok(_mapper.Map<UserResponse>(user));
    }
 
    public async Task<ApiResponse<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        var exists = await _uow.Users.ExistsAsync(u => u.Email == request.Email && !u.IsDeleted, ct);
        if (exists)
            return ApiResponse<UserResponse>.Fail("Email already in use.");
 
        var user = _mapper.Map<User>(request);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
 
        await _uow.Users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);
 
        var savedUser = await _uow.Users.GetWithRoleAsync(user.Id, ct);
        return ApiResponse<UserResponse>.Ok(_mapper.Map<UserResponse>(savedUser), "User created.");
    }
 
    public async Task<ApiResponse<UserResponse>> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetWithRoleAsync(id, ct);
        if (user == null || user.IsDeleted)
            return ApiResponse<UserResponse>.Fail("User not found.");
 
        var emailTaken = await _uow.Users.ExistsAsync(u => u.Email == request.Email && u.Id != id && !u.IsDeleted, ct);
        if (emailTaken)
            return ApiResponse<UserResponse>.Fail("Email already in use.");
 
        _mapper.Map(request, user);
        user.UpdatedAt = DateTime.UtcNow;
 
        await _uow.Users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);
 
        var updated = await _uow.Users.GetWithRoleAsync(id, ct);
        return ApiResponse<UserResponse>.Ok(_mapper.Map<UserResponse>(updated), "User updated.");
    }
 
    public async Task<ApiResponse> DeleteAsync(int id, CancellationToken ct = default)
    {
        var user = await _uow.Users.GetByIdAsync(id, ct);
        if (user == null || user.IsDeleted)
            return ApiResponse.Fail("User not found.");
 
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        await _uow.Users.UpdateAsync(user, ct);
        await _uow.SaveChangesAsync(ct);
 
        return ApiResponse.Ok("User deleted.");
    }
}