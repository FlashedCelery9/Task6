namespace Task6.DTO_s;

public class MeetingAttachmentsDto
{
    /// <summary>
    /// Meeting files id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Files original name
    /// </summary>
    public string OriginalName { get; set; } = null!;
    /// <summary>
    /// Files content type
    /// </summary>
    public string ContentType { get; set; } = null!;
    /// <summary>
    /// Files uploads date
    /// </summary>
    public DateTime UploadedAtUtc { get; set; }

    public string DownloadUrl { get; set; } = null!;
    public int MeetingId {get; set;}
    
}