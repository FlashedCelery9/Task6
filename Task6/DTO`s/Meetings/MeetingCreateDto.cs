using Task6.DTO_s.ParticipantsDto;
using Task6.Models;

namespace Task6.DTO_s.Clients;


public class MeetingCreateDto
{
    /// <summary>
    /// Meetings Participants list
    /// </summary>
    public List<ParticipantDto>? MeetingParticipants { get; set; }
    
    /// <summary>
    /// Meetings title
    /// </summary>
    public string Title { get; set; } = null!;
    
    /// <summary>
    /// Meetings description
    /// </summary>
    public string Description { get; set; } = null!;
    
    /// <summary>
    /// Meetings start time
    /// </summary>
    public DateTime StartTime { get; set; }
    
}