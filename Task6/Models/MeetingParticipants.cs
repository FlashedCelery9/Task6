namespace Task6.Models;

public class MeetingParticipants
{
    public int MeetingId { get; set; }
    public int UserProfileId { get; set; } 
    public UserProfile? UserProfile { get; set; }
    public Meeting? Meeting { get; set; }

    
}