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
        RuleFor(m => m.UserProfilesId)
            .Must(ids => ids.Count >= 0)
            .DependentRules(() =>
            {
                RuleFor(m => m.UserProfilesId)
                    .MustAsync(async (participantId, ct) =>
                    {
                        if (participantId == null || participantId.Count == 0)
                            return true;
                        var existingIds = await db.UserProfiles.Where(p => participantId.Contains(p.Id))
                            .Select(p => p.Id)
                            .ToListAsync(ct);
                        var problem_ids = participantId.Except(existingIds).ToList();
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Participants with id: ");
                        foreach (var id in problem_ids)
                            sb.Append(id).Append(", ");
                        sb.Append("Not found");
                        ErorString += sb.ToString();
                        return existingIds.Count == participantId.Distinct().Count();

                    }).When(m => m.UserProfilesId.Count != null && m.UserProfilesId.Count >= 0)
                    .WithMessage(ErorString);
            });
    }
}