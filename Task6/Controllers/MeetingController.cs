using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task6.data;
using Task6.DTO_s.Clients;
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
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType<IEnumerable<MeetingTitle>>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<MeetingTitle> CreateMeeting(MeetingCreateDto meetingCreate)
    {
        var meeting = _mapper.Map<Meeting>(meetingCreate);
        _context.Add(meeting);
        await _context.SaveChangesAsync();
        return _mapper.Map<MeetingTitle>(meeting);
    }
    /// <summary>
    /// Get sorted meetings by date
    /// </summary>
    /// <returns>List of meetings</returns>
    [HttpGet("bydate")]
    [ProducesResponseType(typeof(IEnumerable<MeetingTitle>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> GetMeetingsByDate(MeetingQueryParameters qp)
    {
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

    public async Task<IActionResult> GetMeetingsByWord([FromQuery] string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return BadRequest("Word cannot be empty.");

        word = word.ToLower();

        var result = await _context.Meetings
            .Where(m => m.Description != null &&
                        m.Description.ToLower().Contains(word))
            .ProjectTo<MeetingTitle>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Ok(result);
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
    public async Task<IActionResult> GetMeetingsByTime([FromQuery] string start, string end)
    {
        DateTime Starttime = DateTime.Parse(start);
        DateTime Endtime = DateTime.Parse(end);
        if (Starttime > Endtime)
        {
            return BadRequest("Start time cannot be greater than End time.");
        }
        var result = await _context.Meetings.Where(m => m.StartTime >= Starttime && m.StartTime <= Endtime)
            .ProjectTo<MeetingDetail>(_mapper.ConfigurationProvider)
            .ToListAsync();
        
        return Ok(result);
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
    

    public async Task<IActionResult> UpdateMeeting(int id, [FromBody] MeetingCreateDto meetingCreateProfile)
    {
        var meeting = _context.Meetings.Where(m => m.Id == id).FirstOrDefault();
        if (meeting == null)
        {
            return NotFound();
        }
        meeting.StartTime = meetingCreateProfile.StartTime;
        meeting.Description = meetingCreateProfile.Description;
        meeting.Title = meetingCreateProfile.Title;
        await  _context.SaveChangesAsync();
        return Ok(meeting);
    }
    /// <summary>
    /// Delete movie
    /// </summary>
    /// <param name="id">id of movie</param>
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
    
    
}