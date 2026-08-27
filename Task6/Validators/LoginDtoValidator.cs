using FluentValidation;
using Task6.data;
using Task6.DTO_s.Identity;

namespace Task6.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator(MeetingsDBContext context)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Passwords must be at least 6 characters long");
    }
}