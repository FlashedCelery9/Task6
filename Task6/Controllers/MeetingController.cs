using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task6.data;
using Task6.DTO_s.Clients;
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

    [HttpGet]
    public async Task<IActionResult> GetMeetings()
    {
        var meetings = await _context.Meetings
            .ProjectTo<MeetingDetail>(_mapper.ConfigurationProvider)
            .ToListAsync();
        return Ok(meetings);
    }

    [HttpPost]
    public async Task<MeetingTitle> setMeeting(MeetingCreateProfile meetingCreateProfile)
    {
        var client = _mapper.Map<Meeting>(meetingCreateProfile);
        _context.Add(client);
        await _context.SaveChangesAsync();
        return _mapper.Map<MeetingTitle>(client);
    }

    [HttpGet("bydate")]
    public async Task<IActionResult> GetMeetingsByDate()
    {
        var result = await _context.Meetings.OrderBy(m => m.StartTime)
            .ProjectTo<MeetingTitle>(_mapper.ConfigurationProvider)
            .ToListAsync();
        return Ok(result);
    }

    [HttpGet("byword")]
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

    [HttpGet("bytime")]
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMeeting(int id, [FromBody] MeetingCreateProfile meetingCreateProfile)
    {
        var meeting = _context.Meetings.Where(m => m.Id == id).FirstOrDefault();
        if (meeting == null)
        {
            return NotFound();
        }
        meeting.StartTime = meetingCreateProfile.StartTime;
        meeting.Description = meetingCreateProfile.Description;
        meeting.Name = meetingCreateProfile.Title;
        await  _context.SaveChangesAsync();
        return Ok(meeting);
    }

    [HttpDelete("{id}")]
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

    [HttpGet("{id}")]
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