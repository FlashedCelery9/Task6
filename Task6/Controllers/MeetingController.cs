using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task6.data;
using Task6.DTO_s.Clients;
using Task6.DTO_s.ParticipantsDto;
using Task6.Helpers.Pagination;
using Task6.Helpers.Queryable;
using Task6.Helpers.QueryParameters;
using Task6.Models;


namespace Task6.Controllers;
[ApiController]
[Route("api/meeting")]

public class MeetingController : ControllerBase
{
    private readonly MeetingsDBContext _context;
    private readonly IMapper _mapper;

    
    public MeetingController(MeetingsDBContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;

    }
    /// <summary>
    /// Get all DetailMeetings
    /// </summary>
    /// <returns>List of meetings</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<MeetingTitle>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMeetings([FromQuery] MeetingQueryParameters qp)
    {
        var query =  _context.Meetings.AsNoTracking()
            .ApplyFilters(qp)
            .ApplySort(qp);
        var dto = await query.ToPagedResultAsync<Meeting, MeetingDetail>(qp.Page, qp.Size, _mapper.ConfigurationProvider);
        return Ok(dto);
    }
    /// <summary>
    /// Create a Meeting
    /// </summary>
    /// <param name="meetingCreate">MeetingCreateDto obj</param>
    /// <returns>Created meeting</returns>
    [HttpPost("meeting")]
    [Consumes("application/json")]
    [ProducesResponseType<IEnumerable<MeetingTitle>>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<MeetingDetail> CreateMeeting([FromBody]MeetingCreateDto meetingCreate)
    {
        var meeting = new Meeting();
        meeting.Title =  meetingCreate.Title;
        meeting.Description =  meetingCreate.Description;
        meeting.StartTime = meetingCreate.StartTime;
        _context.Add(meeting);
        await _context.SaveChangesAsync();
        if (meetingCreate.ParticipantsId.Count > 0)
        {
            foreach (var id in meetingCreate.ParticipantsId)
            {
               _context.MeetingParticipants.Add(new MeetingParticipants{MeetingId = meeting.Id, ParticipantId = id});
            }
        }

        await _context.SaveChangesAsync();
        return _mapper.Map<MeetingDetail>(_context.Meetings.FirstOrDefault(m => m.Id == meeting.Id));
    }
    /// <summary>
    /// Get sorted meetings by date
    /// </summary>
    /// <returns>List of meetings</returns>
    [HttpGet("bydate")]
    [ProducesResponseType(typeof(IEnumerable<MeetingTitle>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> GetMeetingsByDate(int page, int size)
    {
        MeetingQueryParameters qp = new MeetingQueryParameters();
        qp.Sort = "start_time_desc";
        qp.Page = page;
        qp.Size = size;
        var query =  _context.Meetings.AsNoTracking()
            .ApplyFilters(qp)
            .ApplySort(qp);
        var dto = await query.ToPagedResultAsync<Meeting, MeetingDetail>(qp.Page, qp.Size, _mapper.ConfigurationProvider);
        return Ok(dto);
    }

    /// <summary>
    /// Get meetings by word in description
    /// </summary>
    /// <param name="word">word of description</param>
    /// <returns></returns>
    [HttpGet("byword")]
    [ProducesResponseType(typeof(IEnumerable<MeetingTitle>), StatusCodes.Status200OK)]
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

        var result = _context.Meetings
            .ApplyFilters(qp)
            .ApplySort(qp);
        
        var dto = await result.ToPagedResultAsync<Meeting, MeetingTitle>(qp.Page, qp.Size, _mapper.ConfigurationProvider);

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
        var result = _context.Meetings
            .ApplyFilters(qp)
            .ApplySort(qp);
        
        var dto = await result.ToPagedResultAsync<Meeting, MeetingDetail>(qp.Page, qp.Size, _mapper.ConfigurationProvider);
            
        
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
    

    public async Task<IActionResult> UpdateMeeting(MeetingUpdateDto meetingCreateProfile)
    {
        var meeting = _context.Meetings.Where(m => m.Id == meetingCreateProfile.Id).FirstOrDefault();
        if (meeting == null)
        {
            return NotFound();
        }
        meeting.StartTime = meetingCreateProfile.StartTime;
        meeting.Description = meetingCreateProfile.Description;
        meeting.Title = meetingCreateProfile.Title;
        if(meetingCreateProfile.MeetingParticipants != null)
            meeting.MeetingParticipants = meetingCreateProfile.MeetingParticipants;
        await  _context.SaveChangesAsync();
        return Ok(meeting);
    }
    /// <summary>
    /// Delete movie
    /// </summary>
    /// <param name="id">id of meeting</param>
    /// <returns>Deleted movie</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> DeleteMeeting(int id)
    {
        var meet = await _context.Meetings.FindAsync(id);
        if (meet == null)
        {
            return NotFound();
        }

        _context.Remove(meet);
        await _context.SaveChangesAsync();
        return Ok(meet);

    }
    /// <summary>
    /// Get meeting
    /// </summary>
    /// <param name="id">id of meeting</param>
    /// <returns>meeting obj</returns>
    [HttpGet("{id}")]
    [ProducesResponseType<IEnumerable<MeetingTitle>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMeeting(int id)
    {
        var meet = await _context.Meetings.FindAsync(id);
        if (meet != null)
        {
            return Ok(_mapper.Map<MeetingTitle>(meet));
        }
        return NotFound();
    }

    /// <summary>
    /// Create participant
    /// </summary>
    /// <param name="participant">paticipant data</param>
    /// <returns>created participant obj</returns>
    [HttpPost("participant")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePaticipant([FromBody]ParticipantCreateDto dto)
    {
        var participant = new Participant();
        participant.Name = dto.Name; 
        participant.Email = dto.Email;
        _context.Participants.Add(participant);
        await _context.SaveChangesAsync();

        foreach (var id in dto.MeetingsId)
        {
          _context.MeetingParticipants.Add(new MeetingParticipants{MeetingId = id, ParticipantId = participant.Id});
        }
        await _context.SaveChangesAsync();
        return Ok(_mapper.Map<ParticipantCreateDto>(participant));
    }
    
    
}