namespace Task6.Configuration;

public class MeetingsOptions
{
    public const string SectionName = "MeetingOptions";
    
    public int defaultPageSize { get; set; }
    public int defaultPageNumber { get; set; }

}