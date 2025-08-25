using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class JwtTokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessMinutes;
    private readonly int _refreshDays;

    public JwtTokenService(IConfiguration config)
    {
        _config = config;
        _issuer = _config["Jwt:Issuer"]!;
        _audience = _config["Jwt:Audience"]!;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        _accessMinutes = int.Parse(_config["Jwt:AccessTokenMinutes"]!);
        _refreshDays = int.Parse(_config["Jwt:RefreshTokenDays"]!);
    }

    public (string accessToken, DateTime expires) CreateAccessToken(string userId, string? role = null)
    {
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_accessMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        if (!string.IsNullOrWhiteSpace(role))
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: _issuer, audience: _audience,
            claims: claims, expires: expires, signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    public (string token, DateTime expires) CreateRefreshToken()
    {
        // random 256-bit token as Base64
        var bytes = RandomNumberGenerator.GetBytes(32);
        return (Convert.ToBase64String(bytes), DateTime.UtcNow.AddDays(_refreshDays));
    }
}
