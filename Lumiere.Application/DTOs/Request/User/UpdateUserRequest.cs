namespace Lumiere.Application.DTOs.Request.User;

public class UpdateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Country { get; set; }
    public int RoleId { get; set; }
}
