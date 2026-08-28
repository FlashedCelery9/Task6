using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task6.data;
using Task6.DTO_s.Clients;
using Task6.DTO_s.ParticipantsDto;
using Task6.Filters;
using Task6.Helpers.Pagination;
using Task6.Helpers.Queryable;
using Task6.Helpers.QueryParameters;
using Task6.Models;
using Task6.Services;
using Task6.Validators;


namespace Task6.Controllers;
[ApiController]
[Route("api/meeting")]

public class MeetingController(MeetingsDBContext _context,
    IMeetingService _meetingService,
    IMapper _mapper,
    IFileStorageService fileStorage) : ControllerBase
{
    /// <summary>
    /// Pick file to meeting
    /// </summary>
    /// <returns>List of meetings</returns>
    [HttpPost("{id:int}/attachments")]  
    [Consumes("multipart/form-data")]  
    [RequestSizeLimit(1024 * 1024 * 10)]  
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadMeetingFile( [FromRoute] int id, IFormFile file)  
    {        
        var error = FileValidators.ValidateFile(file, 10 * 1024 * 1024);  
        if(!_context.Meetings.AnyAsync(m => m.Id == id).Result) 
            return BadRequest("Meeting not found.");
        if (error is not null)
            return BadRequest(new { error });  
        var dto = await _meetingService.UploadFileAsync(id, file);  
  
        return StatusCode(StatusCodes.Status201Created, dto);  
    }

    /// <summary>
    /// Get all DetailMeetings
    /// </summary>
    /// <returns>List of meetings</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<MeetingDetail>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult>? GetMeetingsAsync([FromQuery] MeetingQueryParameters qp)
    {
        var dto = await _meetingService.GetMeetingsAsync(qp);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);
    }
    
    /// <summary>
    /// Create a Meeting
    /// </summary>
    /// <param name="meetingCreate">MeetingCreateDto obj</param>
    /// <returns>Created meeting</returns>
    [Authorize]
    [HttpPost("meeting")]
    [ServiceFilter(typeof(ValidatorFilter.ValidationFilter<MeetingCreateDto>))]
    [Consumes("application/json")]
    [ProducesResponseType<IEnumerable<MeetingReadDto>>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    
    public async Task<MeetingDetail> CreateMeeting([FromBody]MeetingCreateDto meetingCreate)
    {
        var final_meeting = await _meetingService.CreateMeetingAsync(meetingCreate);
        return final_meeting;
    }
    
    /// <summary>
    /// Get sorted meetings by date
    /// </summary>
    /// <returns>List of meetings</returns>
    [HttpGet("bydate")]
    [ProducesResponseType(typeof(IEnumerable<MeetingReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> GetMeetingsByDate(int page, int size)
    {
        MeetingQueryParameters qp = new MeetingQueryParameters();
        qp.Sort = "start_time_desc";
        qp.Page = page;
        qp.Size = size;
        
        var dto = await _meetingService.GetMeetingsByDateAsync(qp); 
        return Ok(dto);
    }

    /// <summary>
    /// Get meetings by word in description
    /// </summary>
    /// <param name="word">word of description</param>
    /// <returns></returns>
    [HttpGet("byword")]
    [ProducesResponseType(typeof(IEnumerable<MeetingReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> GetMeetingsByWord(int page, int size, string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return BadRequest("Word cannot be empty.");

        word = word.ToLower();
        MeetingQueryParameters qp = new MeetingQueryParameters();
        qp.Page = page;
        qp.Size = size;
        qp.Search_word = word;

        var dto = _meetingService.GetMeetingsByWordAsync(qp);

        return Ok(dto);
    }

    /// <summary>
    /// Get meetings by timeline
    /// </summary>
    /// <param name="start">Start time (from)</param>
    /// <param name="end">End time (to)</param>
    /// <returns>List of meetings</returns>
    [HttpGet("bytime")]
    [ProducesResponseType<IEnumerable<MeetingDetail>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMeetingsByTime(string start, string end, int page, int size)
    {
        MeetingQueryParameters qp = new MeetingQueryParameters();
        qp.Page = page;
        qp.Size = size;
        qp.StartTime = start;
        qp.EndTime = end;

        var dto = _meetingService.GetMeetingsByDateAsync(qp);
        
        return Ok(dto);
    }

    /// <summary>
    /// Update meeting
    /// </summary>
    /// <param name="id">id of movie</param>
    /// <param name="meetingCreateProfile">MeetingCreateDto type obj</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]


    // public async Task<IActionResult> UpdateMeeting([FromRoute] int id, [FromBody] MeetingUpdateDto meetingCreateProfile)
    // {
    //     var dto = await _meetingService.UpdateMeetingAsync(id, meetingCreateProfile);
    //     if (dto == null)
    //     {
    //         return NotFound();
    //     }
    //     return Ok(dto);
    //
    //
    // }

    /// <summary>
    /// Delete movie
    /// </summary>
    /// <param name="id">id of meeting</param>
    /// <returns>Deleted movie</returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> DeleteMeeting(int id)
    {
        var dto = await _meetingService.DeleteMeetingAsync(id);
        if (dto == null)
        {
            return NotFound();
        }
        return Ok(dto);

    }
    /// <summary>
    /// Get meeting
    /// </summary>
    /// <param name="id">id of meeting</param>
    /// <returns>meeting obj</returns>
    [HttpGet("{id}")]
    [ProducesResponseType<IEnumerable<MeetingReadDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMeeting(int id)
    {
        var dto = await _meetingService.GetMeetingByIdAsync(id);
        if (dto == null)
            return NotFound();
        
        return Ok(dto);
    }


    
    
}