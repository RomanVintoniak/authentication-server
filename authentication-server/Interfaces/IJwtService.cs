using System.Security.Claims;

namespace authentication_server.Interfaces
{
    public interface IJwtService
    {
        string GetToken(IEnumerable<Claim> claims, DateTime expirationTime);
        bool IsTokenValid(string token);
        string? GetUserEmailFromToken(string token);
    }
}