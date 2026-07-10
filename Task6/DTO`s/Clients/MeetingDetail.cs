namespace Task6.DTO_s.Clients;

public class MeetingDetail
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartTime { get; set; }
}