namespace Task6.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
}