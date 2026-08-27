using Microsoft.AspNetCore.Identity;
using Task6.DTO_s.Identity;
using Task6.Models;

namespace Task6.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
    {
        var user = new AppUser()
        {
            Email = dto.Email,
            UserName = dto.Email.Split("@")[0]
        };
        return await _userManager.CreateAsync(user, dto.Password);
    }

    public async Task<SignInResult> LoginResult(LoginDto dto)
    {
        var result = await _signInManager.PasswordSignInAsync(dto.Email,
            dto.Password,
            isPersistent: false,
            lockoutOnFailure: true);
        return result;
    }
}