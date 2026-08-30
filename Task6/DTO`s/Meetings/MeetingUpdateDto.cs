using Task6.Models;

namespace Task6.DTO_s.Clients;

public class MeetingUpdateDto
{
    
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