using MediatR;
using Microsoft.AspNetCore.Identity;
using Task6.DTO_s.Auth;
using Task6.DTO_s.Identity;
using Task6.Features.Auth.Commands.Login.DTO_s;
using Task6.Models;
using Task6.Services.Token;

namespace Task6.Features.Auth.Commands.Login;

public record LoginCommand(LoginDto LoginDto) : IRequest<LoginResult>;

public class LoginCommandHandler(
    UserManager<AppUser> userManager,
    SignInManager<AppUser>  signInManager,
    ITokenService tokenService)
    : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var dto = request.LoginDto;
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return new LoginResult(false, false, null);

        var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            return new LoginResult(false, result.IsLockedOut, null);
        }

        var token = await tokenService.CreateAccessTokenAsync(user);
        return new LoginResult(true, false, new AuthResponseDto(AccessToken: token.Token, ExpiresAtUtc: token.ExpiresAtUtc, TokenType: "Bearer"));
    }   
    
}