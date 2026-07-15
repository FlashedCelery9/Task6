namespace Task6.Models;

public class Participant
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public IEnumerable<MeetingParticipants> MeetingParticipants { get; set; }  = new List<MeetingParticipants>();

}