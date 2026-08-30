using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task6.DTO_s.Auth;
using Task6.DTO_s.Identity;
using Task6.Filters;
using Task6.Models;
using Task6.Services;
using Task6.Services.TempServices;
using Task6.Services.Token;

namespace Task6.Controllers;

public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IAuthService _authService;

    public AuthController(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService,
        IAuthService authService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _authService = authService;
    }

    [HttpPost("login", Name = "LoginV1")]
    [AllowAnonymous]
    [ServiceFilter(typeof(ValidationFilter<LoginDto>))]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(
        LoginDto request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Невірний email або пароль",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            user, request.Password, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = result.IsLockedOut
                    ? "Акаунт тимчасово заблоковано"
                    : "Невірний email або пароль",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        var token = await _tokenService.CreateAccessTokenAsync(user, cancellationToken);

        return Ok(new AuthResponseDto(
            AccessToken: token.Token,
            ExpiresAtUtc: token.ExpiresAtUtc,
            TokenType: "Bearer"));
    }

    [HttpPost("register", Name = "registerV1")]
    [ServiceFilter(typeof(ValidationFilter<RegisterDto>))]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterDto request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is not null)
        {
            return BadRequest("This email address already exists");
        }
        
        var result = _authService.RegisterAsync(request);
        
        return Ok(result.Result);

    }
    

    // [HttpGet("me", Name = "GetCurrentUserV1")]
    // [Authorize]
    // public ActionResult<CurrentUserDto> GetCurrentUser()
    // {
    //     var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
    //     var email = User.FindFirstValue(JwtRegisteredClaimNames.Email);
    //     var roles = User.FindAll("role").Select(c => c.Value).ToArray();
    //     var isAdmin = User.IsInRole("Admin");
    //
    //     return Ok(new CurrentUserDto(userId!, email!, roles, isAdmin));
    // }
}