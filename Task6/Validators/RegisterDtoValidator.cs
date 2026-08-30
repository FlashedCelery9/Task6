using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Task6.data;
using Task6.DTO_s.Identity;

namespace Task6.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator(MeetingsDBContext context)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is required")
            .MustAsync(async (email, ct) => !await context.Users.AnyAsync(x => x.Email == email))
            .WithMessage("This email address is already registered");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required").
            MinimumLength(6).WithMessage("Password is minimum 6 characters")
            .Matches(@"[0-9]").WithMessage("Any one number");
        
        RuleFor(x => x.EnabledPassword)
            .NotEmpty().WithMessage("Password is required")
            .Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}