using Microsoft.EntityFrameworkCore;
using Task6.data;
using Task6.DTO_s.Identity;

namespace Task6.Services.TempServices;

public class OldUsersService
{
    private readonly MeetingsDBContext context;

    public OldUsersService(MeetingsDBContext context)
    {
        this.context = context;
    }

    public async Task<bool> OldUserDetector(RegisterDto dto)
    {
        var result = await context.Participants.AnyAsync(p => p.Email == dto.Email);
        if (result)
        {
            return true;
        }
        return false;
    }
}