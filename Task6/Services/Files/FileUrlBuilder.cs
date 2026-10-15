using Microsoft.Extensions.Options;
using Task6.Configuration;

namespace Task6.Services;

public class FileUrlBuilder : IFileUrlBuilder
{
    private readonly string PublicBasePath;  
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly FileStorageOptions _options;
    
    public FileUrlBuilder(IHttpContextAccessor accessor, IOptions<FileStorageOptions> options, IWebHostEnvironment env)
    {
        _options = options.Value;
        PublicBasePath = string.Join("/", options.Value.public_root);
        _httpContextAccessor = accessor;  
    }  
    // public files  
    public string? PublicUrl(string? fileName, string folder)  
    {        if (string.IsNullOrEmpty(fileName)) return null;  
        var request = _httpContextAccessor.HttpContext!.Request;  
        var scheme = request.Scheme; // http  
        var host = request.Host; // localhost:5293  
  
        return $"{scheme}://{host}/{PublicBasePath}/{folder}/{fileName}";  
    }  
  
  
    // Private   
    public string? EndpointUrl(string? fileName)  
    {        throw new NotImplementedException();  
    }  
}