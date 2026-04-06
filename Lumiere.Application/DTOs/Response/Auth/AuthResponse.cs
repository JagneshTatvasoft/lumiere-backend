using Lumiere.Application.DTOs.Response.User;

namespace Lumiere.Application.DTOs.Response.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAt { get; set; }
    public UserResponse User { get; set; } = null!;
}
