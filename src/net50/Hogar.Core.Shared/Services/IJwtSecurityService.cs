using System.Security.Claims;

namespace Bankaool.Core.Shared.Services
{
    public interface IJwtSecurityService
    {
        string GenerateJwtToken(string userId, string role);
        (bool IsValid, bool IsExpired) ValidateToken(string token);
    }
}
