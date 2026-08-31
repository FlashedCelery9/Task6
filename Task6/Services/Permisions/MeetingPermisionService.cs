using Microsoft.EntityFrameworkCore;
using Task6.data;
using Task6.Services.UserService;

namespace Task6.Services;

public class MeetingPermisionService(ICurrentUserService currentUserService, MeetingsDBContext context)
{
    public async Task<bool> IsMeetingAdmin(int meetingId)
    {
        var currentUserId = currentUserService.UserId;
        var meeting = await context.Meetings.FirstOrDefaultAsync(m => m.Id == meetingId);
        if (meeting == null || currentUserId == null)
        {
            return false;
        }
        if(meeting.AdminId == currentUserId)
        {
            return true;
        }
        return false;
    }
}