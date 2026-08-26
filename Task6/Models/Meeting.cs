namespace Task6.Models;

public class Meeting
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; } 
    public DateTime StartTime { get; set; }
    public string? FileName { get; set; }
    public int? RoomId { get; set; }
    public Room? Room { get; set; }

    public ICollection<MeetingAttachment> MeetingAttachments { get; set; }  = new List<MeetingAttachment>();

    public ICollection<MeetingParticipants> MeetingParticipants { get; set; }  = new List<MeetingParticipants>();
}