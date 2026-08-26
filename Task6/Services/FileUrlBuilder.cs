namespace Task6.Services;

public class FileUrlBuilder : IFileUrlBuilder
{
    private const string PublicBasePath = "/uploads";  
    private readonly IHttpContextAccessor _httpContextAccessor;  
    
    public FileUrlBuilder(IHttpContextAccessor accessor)  
    {
        _httpContextAccessor = accessor;  
    }  
    // public files  
    public string? PublicUrl(string? fileName, string folder)  
    {        if (string.IsNullOrEmpty(fileName)) return null;  
        var request = _httpContextAccessor.HttpContext!.Request;  
        var scheme = request.Scheme; // http  
        var host = request.Host; // localhost:5293  
  
        return $"{scheme}://{host}{PublicBasePath}/{folder}/{fileName}";  
    }  
  
  
    // Private   
    public string? EndpointUrl(string? fileName)  
    {        throw new NotImplementedException();  
    }  
}