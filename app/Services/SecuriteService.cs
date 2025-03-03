using Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using System.Runtime;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;


namespace Services;

public class SecuriteService : ISecuriteService
{
    const int keySize = 64;
    const int iterations = 350000;
    byte[] salt;
    HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
    Settings _settings;

    public SecuriteService(IOptions<Settings> settings)
    {
        _settings = settings.Value; 
        salt = Convert.FromHexString(settings.Value.Salt);
    }

    public string HashPasword(string password)
    {
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            hashAlgorithm,
            keySize);
        return Convert.ToHexString(hash);
    }

    public bool VerifyPassword(string password, string hash)
    {
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }

    public string GetJwtSecurityToken(Claim claim)
    {
        var jwt = new JwtSecurityToken(
        issuer: _settings.AuthOptions.ISSUER,
                audience: _settings.AuthOptions.AUDIENCE,
                claims: new List<Claim> { claim },
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.AuthOptions.Key)), SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
