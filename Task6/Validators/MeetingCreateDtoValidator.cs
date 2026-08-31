using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Task6.data;
using Task6.DTO_s.Clients;

namespace Task6.Validators;

public class MeetingCreateDtoValidator : AbstractValidator<MeetingCreateDto>
{
    private string ErorString = " ";

    public MeetingCreateDtoValidator(MeetingsDBContext db)
    {
        RuleFor(m => m.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(30).WithMessage("Title must not exceed 30 characters");
        RuleFor(m => m.StartTime)
            .Must(date => date >= DateTime.Today).WithMessage("Start time must be less than today");

    
    
    }
}