namespace Lumiere.Application.DTOs.Request.User;

public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Country { get; set; }
    public int RoleId { get; set; } = 1;
}
