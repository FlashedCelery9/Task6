using Task6.DTO_s.ParticipantsDto;

namespace Task6.DTO_s.Clients;

public class MeeringReadDto
{
    /// <summary>
    /// Meetings title
    /// </summary>
    public MeetingTitle Title { get; set; }
    
    /// <summary>
    /// Meetings start time
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// Meetings description
    /// </summary>
    public string Description { get; set; } = null!;
    
    /// <summary>
    /// Meetings participants list
    /// </summary>
    public List<ParticipantDto> Participants { get; set; } = new();
}