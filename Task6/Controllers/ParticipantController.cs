using Microsoft.AspNetCore.Mvc;
using Task6.DTO_s.ParticipantsDto;
using Task6.Models;
using Task6.Services;

namespace Task6.Controllers;
[ApiController]
[Route("api/participant")]
public class ParticipantController : ControllerBase
{
    private readonly IParticipantService _participantService;
    public ParticipantController(IParticipantService participantService)
    {
        _participantService = participantService;
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
    public async Task<IActionResult> CreatePaticipant([FromBody]ParticipantCreateDto dto)
    {
        var participant = await _participantService.CreatePaticipant(dto);
        return Ok(participant);
    }
    
}