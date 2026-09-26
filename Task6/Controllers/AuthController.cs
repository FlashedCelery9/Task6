using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task6.DTO_s.Auth;
using Task6.DTO_s.Identity;
using Task6.Features.Auth.Commands.Login;
using Task6.Features.Auth.Commands.Register;
using Task6.Filters;
using Task6.Models;
using Task6.Services;
using Task6.Services.TempServices;
using Task6.Services.Token;

namespace Task6.Controllers;

public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IMediator _mediator;

    public AuthController(UserManager<AppUser> userManager,
        IMediator mediator)
    {
        _userManager = userManager;
  
        _mediator = mediator;
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
        var result = await _mediator.Send(new LoginCommand(request), cancellationToken);

        if (!result.Success)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = result.IsLockedOut
                    ? "Акаунт тимчасово заблоковано"
                    : "Невірний email або пароль",
                Status = StatusCodes.Status401Unauthorized
            });
        }
        return Ok(result.Response);
    }

    [HttpPost("register", Name = "registerV1")]
    [ServiceFilter(typeof(ValidationFilter<RegisterDto>))]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterDto request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is not null)
        {
            return BadRequest("This email address already exists");
        }

        var result = await _mediator.Send(new RegisterCommand(request), ct);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        return Ok();

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