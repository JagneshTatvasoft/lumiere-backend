using AutoMapper;
using Lumiere.Application.DTOs.Request.Auth;
using Lumiere.Application.DTOs.Response;
using Lumiere.Application.DTOs.Response.Auth;
using Lumiere.Application.DTOs.Response.User;
using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Application.Interfaces.Services;
using Lumiere.Domain.Entities;

namespace Lumiere.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(IUnitOfWork uow, ITokenService tokenService, IMapper mapper)
    {
        _uow = uow;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        
        var user = await _uow.Users.GetByEmailAsync(request.Email, ct);
        if (user == null || user.IsDeleted)
            return ApiResponse<AuthResponse>.Fail("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return ApiResponse<AuthResponse>.Fail("Invalid email or password.");

        var userWithRole = await _uow.Users.GetWithRoleAsync(user.Id, ct);
        var token = _tokenService.GenerateToken(user.Id, user.Email, userWithRole!.Role.Name);
        var expires = _tokenService.GetExpiration();

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            Token = token,
            ExpiresAt = expires,
            User = _mapper.Map<UserResponse>(userWithRole)
        }, "Login successful.");
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var exists = await _uow.Users.ExistsAsync(u => u.Email == request.Email && !u.IsDeleted, ct);
        if (exists)
            return ApiResponse<AuthResponse>.Fail("Email is already registered.");

        var userRole = await _uow.Roles.GetByNameAsync("User", ct);
        if (userRole == null)
            return ApiResponse<AuthResponse>.Fail("Role configuration error.");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Country = request.Country,
            RoleId = userRole.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        var savedUser = await _uow.Users.GetWithRoleAsync(user.Id, ct);
        var token = _tokenService.GenerateToken(user.Id, user.Email, savedUser!.Role.Name);

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            Token = token,
            ExpiresAt = _tokenService.GetExpiration(),
            User = _mapper.Map<UserResponse>(savedUser)
        }, "Registration successful.");
    }

}
