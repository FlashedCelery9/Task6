using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task6.data;
using Task6.DTO_s.Clients;
using Task6.Helpers.Pagination;
using Task6.Helpers.Queryable;
using Task6.Helpers.QueryParameters;
using Task6.Models;
using Task6.Services.UserService;

namespace Task6.Services;

public class MeetingService(
    MeetingsDBContext context,
    IMapper mapper,
    IFileStorageService fileStorageService,
    IFileUrlBuilder fileUrlBuilder,
    ICurrentUserService currentUserService
    ) : IMeetingService
{
    private readonly string AttachmentFolder = "MeetingAttachments";
    
    public async Task<PagedResult<MeetingDetail>>? GetMeetingsAsync([FromQuery] MeetingQueryParameters qp)
    {
        var query =  context.Meetings.AsNoTracking()
            .ApplyFilters(qp)
            .ApplySort(qp)
            ;
        var dto = await query.ToPagedResultAsync<Meeting, MeetingDetail>(qp.Page, qp.Size, mapper.ConfigurationProvider);
        return dto;
    }

    public async Task<MeetingDetail>? CreateMeetingAsync([FromBody] MeetingCreateDto meetingCreate)
    {
        var meeting = new Meeting();
        meeting.Title = meetingCreate.Title;
        meeting.StartTime = meetingCreate.StartTime;
        meeting.Description = meetingCreate.Description;
        meeting.AdminId = currentUserService.UserId;
        context.Meetings.Add(meeting);
        
        await context.SaveChangesAsync();
    
        var final_res = await context.Meetings.Include(m => m.MeetingParticipants)
            .ThenInclude(mp => mp.UserProfile)
            .ThenInclude(mp => mp.User)
            .Include(m => m.Admin)
            .FirstOrDefaultAsync(m => m.Id == meeting.Id)
            ;
        return mapper.Map<MeetingDetail>(final_res);

    }

    public async Task<PagedResult<MeetingDetail>> GetMeetingsByDateAsync(MeetingQueryParameters qp)
    {
        var query = context.Meetings.Include(m => m.Admin).AsNoTracking()
            .ApplyFilters(qp)
            .ApplySort(qp);

        var res = await query.ToPagedResultAsync<Meeting,MeetingDetail>(qp.Page, qp.Size, mapper.ConfigurationProvider);
        return res;
    }

    public async Task<PagedResult<MeetingReadDto>>? GetMeetingsByWordAsync(MeetingQueryParameters qp)
    {
        var query = context.Meetings.AsNoTracking()
            .ApplyFilters(qp)
            .ApplySort(qp);

        var res = await query.ToPagedResultAsync<Meeting,  MeetingReadDto>(qp.Page, qp.Size, mapper.ConfigurationProvider);
        return res;

    }

    public async Task<PagedResult<MeetingDetail>> GetMeetingsByTimeAsync(MeetingQueryParameters qp)
    {
        var query = context.Meetings.Include(m => m.MeetingAttachments)
            .Include(m => m.Admin)
            .AsNoTracking()
            .ApplyFilters(qp)
            .ApplySort(qp);

        var res = await query.ToPagedResultAsync<Meeting, MeetingDetail>(qp.Page, qp.Size, mapper.ConfigurationProvider);
        return res;
    }

    public async Task<MeetingDetail?> UpdateMeetingAsync(int id, MeetingUpdateDto meetingUpdate)
    {
        var meeting = await context.Meetings.FirstOrDefaultAsync(m => m.Id == id);
        if (meeting == null)
        {
            return null;
        }
        meeting.Title = meetingUpdate.Title;
        meeting.Description = meetingUpdate.Description;
        meeting.StartTime = meetingUpdate.StartTime;
        meeting.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return  mapper.Map<MeetingDetail>(meeting);




}

    public async Task<MeetingDetail?> DeleteMeetingAsync(int id)
    {
        var meet = await context.Meetings.Include(m => m.Admin)
            .Include(m => m.MeetingParticipants)
            .ThenInclude(mp => mp.UserProfile)
            .ThenInclude(up => up.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (meet == null)
        {
            return null;
        }

        context.Remove(meet);
        await context.SaveChangesAsync();
        return mapper.Map<MeetingDetail>(meet);
    }

    public async Task<MeetingDetail?> GetMeetingByIdAsync(int id)
    {
        var meet = await context.Meetings
            .Include(m => m.MeetingParticipants)
            .ThenInclude(mp => mp.UserProfile)
            .ThenInclude(mp => mp.User)
            .Include(m => m.MeetingAttachments)
            .Include(m => m.Admin)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (meet != null)
        {
            return mapper.Map<MeetingDetail>(meet);
        }
        return null;
    }

    public async Task<MeetingDetail?> UploadFileAsync(int id, IFormFile file)
    {
        var meeting = await context.Meetings.FirstOrDefaultAsync(m => m.Id == id);
        if (meeting == null) return null;
        
        MeetingAttachment meetingAttachment = new MeetingAttachment();
        meetingAttachment.MeetingId = id;
        meetingAttachment.OriginalName = file.FileName;
        meetingAttachment.ContentType = file.ContentType;
        var fileName = await fileStorageService.SaveAsync(file, "MeetingFiles", FileVisibility.Public);
        if (meeting.FileName != null)
        {
            fileStorageService.Delete("MeetingFiles", meeting.FileName, FileVisibility.Public);
        }    
        meeting.FileName = fileName.FileName;
        meetingAttachment.StoredFileName = meeting.FileName;
        meetingAttachment.UploadedAtUtc = DateTime.UtcNow;
        context.MeetingAttachments.Add(meetingAttachment);
        
        await context.SaveChangesAsync();
        
        var dto = mapper.Map<MeetingDetail>(meeting);
        dto.Filename = fileUrlBuilder.PublicUrl(meeting.FileName, "MeetingFiles");;
        
        return dto;
    }
    public async Task<int?> AddAttachmentAsync(int meetingId, IFormFile file, CancellationToken ct = default)
    {
        var meeting = await context.Meetings.FindAsync([meetingId], ct);
        if (meeting is null) return null;

        var stored = await fileStorageService.SaveAsync(file, AttachmentFolder, FileVisibility.Private);

        var attachment = new MeetingAttachment
        {
            MeetingId = meetingId,
            StoredFileName = stored.FileName,
            OriginalName = stored.OriginalFileName
            ,ContentType = file.ContentType
        };
        context.MeetingAttachments.Add(attachment);
        await context.SaveChangesAsync();

        return attachment.Id;
    }
    public async Task<FileDownload?> GetAttachmentAsync(int attachmentId, CancellationToken ct = default)
    {
        var att = await context.MeetingAttachments.FindAsync([attachmentId], ct);
        if (att is null) return null;

        var download = await fileStorageService.OpenRead(AttachmentFolder, att.StoredFileName, FileVisibility.Private);
        if (download is null) return null;
        
        return download with
        {
            DownloadName = att.OriginalName,
            ContentType = download.ContentType
        };
    }
    // public async Task<MeetingDetail?> AddParticipant(string email, int meetingid)
    // {
    //     var participant = await userManager.FindByEmailAsync(email);
    //     if (participant == null)
    //     {
    //         return null;
    //     }
    //     var meetingparticipant = new MeetingParticipants{MeetingId = meetingid, UserProfileId = participant.Id};
    //     context.MeetingParticipants.Add(meetingparticipant);
    //     context.SaveChangesAsync();
    //     return mapper.Map<MeetingDetail>(meetingparticipant);
    // }

}