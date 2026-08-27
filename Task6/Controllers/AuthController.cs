using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task6.DTO_s.Identity;
using Task6.Services;
using Task6.Services.TempServices;

namespace Task6.Controllers;

public class AuthController(IAuthService _authService, OldUsersService oldUserDetector) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<IdentityResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var detector = await oldUserDetector.OldUserDetector(dto);
        if(detector) return BadRequest("User already exists(Participant table!!!!)");
        var result = await _authService.RegisterAsync(dto);
        if (!result.Succeeded) return BadRequest(result.Errors);  
        return Ok(result); 
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.LoginResult(dto);
        
        if(result.IsLockedOut) return BadRequest("User locked out");
        if (!result.Succeeded) return Unauthorized("Invalid username or password");
        return Ok(result);
    }
}