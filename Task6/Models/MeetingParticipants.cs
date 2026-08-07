namespace Task6.Models;

public class MeetingParticipants
{
    public int MeetingId { get; set; }
    public int ParticipantId { get; set; }
    public Participant? Participant { get; set; }
    public Meeting? Meeting { get; set; }

    
}