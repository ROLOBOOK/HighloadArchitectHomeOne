
using System.Security.Claims;

namespace Services;

public interface ISecuriteService
{
    public string HashPasword(string password);
    public bool VerifyPassword(string password, string hash);
    public string GetJwtSecurityToken(Claim claim);
}
