using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcommerceAPI.Core.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceAPI.Services.Identity;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config) => _config = config;

    public string CreateToken(IEnumerable<Claim> claims, DateTime expiresAtUtc)
    {
        var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing.");
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];

        // Ensure we always include NameIdentifier alongside sub for compatibility
        var claimsList = claims.ToList();
        var sub = claimsList.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (!string.IsNullOrWhiteSpace(sub) && claimsList.All(c => c.Type != ClaimTypes.NameIdentifier))
        {
            claimsList.Add(new Claim(ClaimTypes.NameIdentifier, sub));
        }

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claimsList,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
