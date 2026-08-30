using Task6.Models;
namespace Task6.DTO_s.Clients;
using Task6.DTO_s.ParticipantsDto;


public class MeetingDetail
{
    /// <summary>
    /// Meetings id
    /// </summary>
    public int Id { get; set; }
    
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
    /// <summary>
    /// Meeting updated at 
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    public List<MeetingAttachmentsDto?> MeetingAttachments { get; set; }
    public string? Filename { get; set; }
    
    /// <summary>
    /// meetings room
    /// </summary>
    public Room? Room { get; set; }
    
    /// <summary>
    /// Meetings participants list
    /// </summary>
    public List<ParticipantDto>? MeetingParticipants { get; set; } = new();
    public MeetingAdminDto MeetingAdmin { get; set; } 
}