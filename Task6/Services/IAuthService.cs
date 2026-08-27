using Microsoft.AspNetCore.Identity;
using Task6.DTO_s.Identity;

namespace Task6.Services;

public interface IAuthService
{
    Task<IdentityResult> RegisterAsync(RegisterDto dto);  
    Task<SignInResult> LoginResult(LoginDto dto);  
}