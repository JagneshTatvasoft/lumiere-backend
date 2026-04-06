namespace Lumiere.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(int userId, string email, string role);
    DateTime GetExpiration();
}
