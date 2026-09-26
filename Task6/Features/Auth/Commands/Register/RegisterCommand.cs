using MediatR;
using Microsoft.AspNetCore.Identity;
using Task6.data;
using Task6.DTO_s.Identity;
using Task6.Models;

namespace Task6.Features.Auth.Commands.Register;

public record RegisterCommand(RegisterDto Dto) : IRequest<IdentityResult>;
//Повідомлення для MediatR що це команда в реалізації якої є метод Handler який повертатиме вказаний клас там

public class RegisterCommandHandler(UserManager<AppUser> _userManager, MeetingsDBContext _context)
    : IRequestHandler<RegisterCommand, IdentityResult>
{
    public async Task<IdentityResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var user = new AppUser()
        {
            Email = dto.Email,
            UserName = dto.Email.Split('@')[0]
        };
        var result = await _userManager.CreateAsync(user, dto.Password);
        await _context.SaveChangesAsync(cancellationToken);
        if (!result.Succeeded)
        {
            return result;
        }

        var UserProfile = new UserProfile()
        {
            UserId = user.Id
        };
        _context.UserProfiles.Add(UserProfile);
        await _context.SaveChangesAsync();
        return IdentityResult.Success;
        
    }
} 