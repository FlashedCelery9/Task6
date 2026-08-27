using Microsoft.AspNetCore.Identity;

namespace Task6.Models;

public class UserProfile
{
    public int Id { get; set; }
    
    public string UserId { get; set; }
    public AppUser User { get; set; }
    
    public ICollection<MeetingParticipants> MeetingParticipants { get; set; }  = new List<MeetingParticipants>();
}