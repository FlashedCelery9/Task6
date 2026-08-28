using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Task6.Models;

namespace Task6.Services.Token;

public class JwtTokenService : ITokenService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration; 
    public JwtTokenService(
        UserManager<AppUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }
    public async Task<AccessTokenResult> CreateAccessTokenAsync(
        AppUser user,
        CancellationToken cancellationToken = default)
    {
        var jwt = _configuration.GetSection("Jwt");

        var issuer   = jwt["Issuer"]   ?? throw new InvalidOperationException("Jwt:Issuer не налаштовано");
        var audience = jwt["Audience"] ?? throw new InvalidOperationException("Jwt:Audience не налаштовано");
        var key      = jwt["Key"]      ?? throw new InvalidOperationException("Jwt:Key не налаштовано");
        var minutes  = jwt.GetValue<int?>("AccessTokenMinutes") ?? 15;

        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(minutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new("username", user.UserName!),
            new("email_verified", user.EmailConfirmed ? "true" : "false")
        };

        // Ролі з Identity (Модуль 13) → claims у токені
        foreach (var role in await _userManager.GetRolesAsync(user))
        {
            claims.Add(new Claim("role", role));
        }

        // Персональні claims, збережені через _userManager.AddClaimAsync(...)
        claims.AddRange(await _userManager.GetClaimsAsync(user));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = now,
            NotBefore = now,
            Expires = expiresAt,
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler();
        var token = handler.CreateToken(descriptor);

        return new AccessTokenResult(token, expiresAt);
    }
    
}