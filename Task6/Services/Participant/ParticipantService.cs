using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task6.data;
using Task6.DTO_s.Clients;
using Task6.DTO_s.ParticipantsDto;
using Task6.Models;

namespace Task6.Services.Participant;

public class ParticipantService(MeetingsDBContext context, IMapper mapper, UserManager<AppUser> userManager) : IParticipantService
{
    public async Task<MeetingDetail?> AddParticipantToMeetingAsync(ParticipantToMeetingDto meetingDto)
    {
        // 1. Знаходимо користувача
        var appUser = await userManager.FindByEmailAsync(meetingDto.UserEmail);
        if (appUser == null)
            return null;

        // 2. Знаходимо мітинг
        var meeting = await context.Meetings
            .Include(m => m.Admin)
            .Include(m => m.MeetingParticipants)
            .ThenInclude(mp => mp.UserProfile)
            .ThenInclude(u => u.User)
            .Include(m => m.MeetingAttachments)
            .FirstOrDefaultAsync(m => m.Id == meetingDto.MeetingId);

        if (meeting == null)
            return null;

        // 3. Знаходимо профіль користувача
        var userProfile = await context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == appUser.Id);

        if (userProfile == null)
            return null;

        // 4. Перевіряємо, чи вже є учасник
        var exists = meeting.MeetingParticipants
            .Any(mp => mp.UserProfileId == userProfile.Id);

        if (!exists)
        {
            meeting.MeetingParticipants.Add(new MeetingParticipants
            {
                MeetingId = meeting.Id,
                UserProfileId = userProfile.Id
            });

            await context.SaveChangesAsync();
        }

        // 5. Повертаємо MeetingDetail
        return mapper.Map<MeetingDetail>(meeting);
    }

    public async Task<MeetingDetail?> RemoveParticipantFromMeetingAsync(ParticipantToMeetingDto meetingDto)
    {
        var user = await userManager.FindByEmailAsync(meetingDto.UserEmail);
        if (user == null)
        {
            return null;
        }

        var meeting = await context.Meetings
            .Include(m => m.Admin)
            .Include(m => m.MeetingParticipants)
            .ThenInclude(mp => mp.UserProfile)
            .ThenInclude(u => u.User)
            .FirstOrDefaultAsync(m => m.Id == meetingDto.MeetingId);
        if (meeting == null)
        {
            return null;
        }
        var userProfile = await context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == user.Id);

        if (userProfile == null)
            return null;
        
        var exists = meeting.MeetingParticipants
            .Any(mp => mp.UserProfileId == userProfile.Id);
        if (!exists)
        {
            return null;
        }


        var mpToDelete = await context.MeetingParticipants
            .FirstOrDefaultAsync(mp =>
                mp.MeetingId == meetingDto.MeetingId &&
                mp.UserProfile.User.Email == meetingDto.UserEmail);

        if (mpToDelete == null)
        {
            return null; // або false, або кинути помилку — як тобі треба
        }        
        context.MeetingParticipants.Remove(mpToDelete);
        await context.SaveChangesAsync();
        return mapper.Map<MeetingDetail>(meeting);
    }

}