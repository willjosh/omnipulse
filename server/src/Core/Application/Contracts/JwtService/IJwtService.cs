namespace Application.Contracts.JwtService;

public interface IJwtService
{
    string GenerateToken(string userId, string email, IList<string> roles);
    string GenerateRefreshToken();
}