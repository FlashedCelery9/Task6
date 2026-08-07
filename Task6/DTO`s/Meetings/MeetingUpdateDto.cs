using Task6.Models;

namespace Task6.DTO_s.Clients;

public class MeetingUpdateDto
{
    /// <summary>
    /// Meetings id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Meetings Participants list
    /// </summary>
    public ICollection<MeetingParticipants>? MeetingParticipants { get; set; }
    
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