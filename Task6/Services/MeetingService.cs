using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task6.data;
using Task6.DTO_s.Clients;
using Task6.Helpers.Pagination;
using Task6.Helpers.Queryable;
using Task6.Helpers.QueryParameters;
using Task6.Models;

namespace Task6.Services;

public class MeetingService(
    MeetingsDBContext context,
    IMapper mapper,
    IFileStorageService fileStorageService,
    IFileUrlBuilder fileUrlBuilder) : IMeetingService
{
    
    
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
        
        context.Meetings.Add(meeting);
        
        await context.SaveChangesAsync();
        if (meetingCreate.ParticipantsId.Count > 0)
        {
            foreach (var pid in meetingCreate.ParticipantsId)
            {
                if (context.Participants.Any(p => p.Id == pid))
                {
                    context.MeetingParticipants.Add(new MeetingParticipants{MeetingId = meeting.Id, UserProfileId = pid});
                }
            }

            await context.SaveChangesAsync();
          
        }
        var final_res = await context.Meetings.Include(m => m.MeetingParticipants)
            .ThenInclude(mp => mp.UserProfile)
            .FirstOrDefaultAsync(m => m.Id == meeting.Id);
        return mapper.Map<MeetingDetail>(final_res);

    }

    public async Task<PagedResult<MeetingDetail>> GetMeetingsByDateAsync(MeetingQueryParameters qp)
    {
        var query = context.Meetings.AsNoTracking()
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
        var query = context.Meetings.AsNoTracking()
            .ApplyFilters(qp)
            .ApplySort(qp);

        var res = await query.ToPagedResultAsync<Meeting, MeetingDetail>(qp.Page, qp.Size, mapper.ConfigurationProvider);
        return res;
    }

    public async Task<MeetingDetail> UpdateMeetingAsync(int id, MeetingUpdateDto meetingUpdate)
    {
        var meeting = await context.Meetings.Include(m => m.MeetingParticipants).FirstOrDefaultAsync(m=> m.Id == id);

        if (meeting == null)
        {
            return null;
        }
        meeting.Title = meetingUpdate.Title;
        meeting.Description = meetingUpdate.Description;
        meeting.StartTime = meetingUpdate.StartTime;

        if (meetingUpdate.ParticipantsId.Count == 0)
        {
            await context.SaveChangesAsync();
            return mapper.Map<MeetingDetail>(meeting);
        }
        var currentParticipants = meeting.MeetingParticipants.Select(mp => mp.UserProfileId).ToList();
        
        var to_delete = currentParticipants.Except(meetingUpdate.ParticipantsId).ToList();
        var toAddIds = meetingUpdate.ParticipantsId.Except(currentParticipants).ToList();

        
        //Deleting
        foreach (var el in to_delete)
        {
            var el_to_del = meeting.MeetingParticipants
                .FirstOrDefault(mp => mp.MeetingId == id && mp.UserProfileId == el);
            
            context.MeetingParticipants.Remove(el_to_del);
            
        }

        foreach (var el in toAddIds)
        {
            if(!context.Participants.Any(p => p.Id == el))
                continue;

            var el_to_add = new MeetingParticipants{MeetingId = id, UserProfileId = el};
            
            context.MeetingParticipants.Add(el_to_add);
        }
        await context.SaveChangesAsync();
        var res = await context.Meetings.Include(m => m.MeetingParticipants)
            .ThenInclude(mp => mp.UserProfile)
            .FirstOrDefaultAsync(m => m.Id == id);
        return mapper.Map<MeetingDetail>(res);
        
        
    }

    public async Task<MeetingDetail> DeleteMeetingAsync(int id)
    {
        var meet = await context.Meetings.FindAsync(id);
        if (meet == null)
        {
            return null;
        }

        context.Remove(meet);
        await context.SaveChangesAsync();
        return mapper.Map<MeetingDetail>(meet);
    }

    public async Task<MeetingDetail> GetMeetingByIdAsync(int id)
    {
        var meet = await context.Meetings
            .Include(m => m.MeetingParticipants)
            .ThenInclude(mp => mp.UserProfile)
            .Include(m => m.MeetingAttachments)
            .FirstOrDefaultAsync(m => m.Id == id); 
        if (meet != null)
        {
            return mapper.Map<MeetingDetail>(meet);
        }
        return null;
    }

    public async Task<MeetingReadDto?> UploadFileAsync(int id, IFormFile file)
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
        meeting.FileName = fileName;
        meetingAttachment.StoredFileName = meeting.FileName;
        meetingAttachment.UploadedAtUtc = DateTime.UtcNow;
        context.MeetingAttachments.Add(meetingAttachment);
        
        await context.SaveChangesAsync();
        
        var dto = mapper.Map<MeetingReadDto>(meeting);
        dto.FileName = fileUrlBuilder.PublicUrl(meeting.FileName, "MeetingFiles");;
        
        return dto;
    }
}