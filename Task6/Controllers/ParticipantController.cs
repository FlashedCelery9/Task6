using Microsoft.AspNetCore.Mvc;
using Task6.DTO_s.ParticipantsDto;
using Task6.Models;
using Task6.Services;
using Task6.Services.Participant;

namespace Task6.Controllers;
[ApiController]
[Route("api/participant")]
public class ParticipantController : ControllerBase
{
    private readonly IParticipantService _participantService;
    private readonly MeetingPermisionService permisionService;
    public ParticipantController(IParticipantService participantService, MeetingPermisionService permisionService)
    {
        _participantService = participantService;
        this.permisionService = permisionService;
    }

    /// <summary>
    /// Create participant
    /// </summary>
    /// <param name="participant">paticipant data</param>
    /// <returns>created participant obj</returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePaticipant([FromBody] ParticipantToMeetingDto dto)
    {
        if (!await permisionService.IsMeetingAdmin(dto.MeetingId))
        {
            return BadRequest("You do not have permission to access this meeting");
        }
        var participant = await _participantService.AddParticipantToMeetingAsync(dto);
        if (participant == null)
        {
            return BadRequest("Participant or meeting not found");
        }
    return Ok(participant);
    }
    
    /// <summary>
    /// Remove participant
    /// </summary>
    /// <param name="participant">paticipant data</param>
    /// <returns>created participant obj</returns>
    [HttpDelete]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveParticipant([FromBody] ParticipantToMeetingDto dto)
    {
        if (!await permisionService.IsMeetingAdmin(dto.MeetingId))
        {
            return BadRequest("You do not have permission to access this meeting");
        }
        var participant = await _participantService.RemoveParticipantFromMeetingAsync(dto);
        if (participant == null)
        {
            return BadRequest("Participant or meeting not found");
        }
        return Ok(participant);
    }
    
}