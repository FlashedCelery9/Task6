using Microsoft.AspNetCore.Identity;
using Task6.data;
using Task6.DTO_s.Identity;
using Task6.Models;

namespace Task6.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly MeetingsDBContext _context;

    public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,  MeetingsDBContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }
    public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
    {
        var user = new AppUser()
        {
            Email = dto.Email,
            UserName = dto.Email.Split("@")[0]
        };
        var result = await _userManager.CreateAsync(user, dto.Password);
        var userProfile = new UserProfile();
        var userRes = await _userManager.FindByEmailAsync(user.Email);
        userProfile.UserId = userRes.Id;
        _context.UserProfiles.Add(userProfile);
        await _context.SaveChangesAsync();
        return result;
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