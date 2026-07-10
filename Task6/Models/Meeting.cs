namespace Task6.Models;

public class Meeting
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; } 
    public DateTime StartTime { get; set; }
}