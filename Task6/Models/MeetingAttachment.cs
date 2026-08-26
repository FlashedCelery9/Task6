namespace Task6.Models;

public class MeetingAttachment
{
   
    public int Id {get; set;}

    public string OriginalName { get; set; } = null!;

    public string StoredFileName { get; set; } = null!;

    public string ContentType { get; set; } = null!;
    
    public DateTime UploadedAtUtc { get; set; }
    
    public int MeetingId { get; set; }
    public Meeting Meeting { get; set; } = null!;

}