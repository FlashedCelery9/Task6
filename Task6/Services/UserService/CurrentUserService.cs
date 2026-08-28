using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Task6.Services.UserService;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;

    private ClaimsPrincipal? User => _accessor.HttpContext?.User;

    public string? UserId => User?.FindFirstValue(JwtRegisteredClaimNames.Sub);
    public string? Email  => User?.FindFirstValue(JwtRegisteredClaimNames.Email);
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
}