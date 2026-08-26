using Microsoft.AspNetCore.Mvc;
using Task6.DTO_s.Clients;
using Task6.Helpers.Pagination;
using Task6.Helpers.QueryParameters;

namespace Task6.Services;

public interface IMeetingService
{ 
    Task<PagedResult<MeetingDetail>>? GetMeetingsAsync([FromQuery] MeetingQueryParameters qp);
    Task<MeetingDetail>? CreateMeetingAsync([FromBody]MeetingCreateDto meetingCreate);
    Task<PagedResult<MeetingDetail>> GetMeetingsByDateAsync(MeetingQueryParameters qp);
    Task<PagedResult<MeetingReadDto>>? GetMeetingsByWordAsync(MeetingQueryParameters qp);
    Task<PagedResult<MeetingDetail>> GetMeetingsByTimeAsync(MeetingQueryParameters qp);
    Task<MeetingDetail> UpdateMeetingAsync(int id, MeetingUpdateDto meetingUpdate);
    Task<MeetingDetail> DeleteMeetingAsync(int id);
    Task<MeetingDetail> GetMeetingByIdAsync(int id);
    Task<MeetingReadDto> UploadFileAsync(int id, IFormFile file);
}