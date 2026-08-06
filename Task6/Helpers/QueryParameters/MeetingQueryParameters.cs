namespace Task6.Helpers.QueryParameters;

public class MeetingQueryParameters
{
    public int Page { get; set; }
    public int Size { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Search_word { get; set; }
    public string? Sort { get; set; }
}