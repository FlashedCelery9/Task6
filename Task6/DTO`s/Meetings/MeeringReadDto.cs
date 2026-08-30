using Task6.DTO_s.ParticipantsDto;

namespace Task6.DTO_s.Clients;

public class MeetingReadDto
{
    /// <summary>
    /// Meetings title
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// Meetings start time
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// Meetings description
    /// </summary>
    public string Description { get; set; } = null!;


}