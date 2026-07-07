using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TodoList.Api.Domain.Entities;
using TodoList.Api.Shared.Domain.Providers;

namespace TodoList.Api.Shared.Infrastructure.Providers;

public class TokenProvider(IConfiguration configuration) : ITokenProvider
{
    private readonly IConfiguration _configuration = configuration;

    public (string Token, DateTime ExpiresAt) GenerateAccessToken(User user)
    {
        var secretKey = _configuration["JWT_SECRET"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT_SECRET is not configured.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var issuer = _configuration["JWT_ISSUER"];
        var audience = _configuration["JWT_AUDIENCE"];
        var expiryMinutesStr = _configuration["JWT_EXPIRY_MINUTES"] ?? "60";
        var expiryMinutes = double.Parse(expiryMinutesStr);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public (string Token, DateTime ExpiresAt) GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var token = Convert.ToBase64String(randomNumber);

        var refreshExpiryDaysStr = _configuration["JWT_REFRESH_EXPIRY_DAYS"] ?? "7";
        var refreshExpiryDays = double.Parse(refreshExpiryDaysStr);
        var expiresAt = DateTime.UtcNow.AddDays(refreshExpiryDays);

        return (token, expiresAt);
    }
}
