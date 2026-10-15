namespace Task6.Configuration;

public class FileStorageOptions
{
    public const string SectionName = "FileStorage";
    
    public List<string> public_root { get; set; } = new List<string>();
    public List<string> private_root { get; set; } = new();
    public string defaultFolder { get; set; } = "General";

}