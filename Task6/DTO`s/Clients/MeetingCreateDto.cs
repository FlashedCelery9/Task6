using Task6.Models;

namespace Task6.DTO_s.Clients;


public class MeetingCreateDto
{
    public List<Participant> Participants { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartTime { get; set; }
}