using Task6.Models;

namespace Task6.Services.Token;

public interface ITokenService
{
    Task<AccessTokenResult> CreateAccessTokenAsync(AppUser user, CancellationToken cancellationToken = default);
}
public sealed record AccessTokenResult(string Token, DateTime ExpiresAtUtc);