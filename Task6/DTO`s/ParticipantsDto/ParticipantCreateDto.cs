using Task6.Models;

namespace Task6.DTO_s.ParticipantsDto;

public class ParticipantCreateDto
{
    /// <summary>
    /// Participants name
    /// </summary>
    public string Name { get; set; } = null!;
    /// <summary>
    /// Partisipants Email
    /// </summary>
    public string? Email { get; set; }
    /// <summary>
    /// Participants meetings
    /// </summary>
    public List<int> MeetingsId { get; set; }  = new List<int>();

    
}