using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Task6.Configuration;

namespace Task6.Services;

public class OptionalFileStrorageService : IFileStorageService
{
    private FileStorageOptions _options;
    private readonly string _publicRoot;
    private readonly string _privateRoot;
  
    public OptionalFileStrorageService(IWebHostEnvironment env, IOptions<FileStorageOptions> options)
    {
        _options = options.Value;
        var publicParts = _options.public_root.Count > 0
            ? _options.public_root
            : _options.public_root.Append("PublicFolder");

        var privateParts = _options.private_root.Count > 0 ? _options.private_root
            : _options.private_root.Append("PrivateFolder");
        _publicRoot = env.ContentRootPath;
        _privateRoot = env.ContentRootPath;
     
        foreach (var file in publicParts)
        {
            _publicRoot =  Path.Combine(_publicRoot, file); 
        }

        foreach (var file in privateParts)
        {
            _privateRoot = Path.Combine(_privateRoot, file);
        }
    }
    private string RootFor(FileVisibility visibility) => visibility == FileVisibility.Public ? _publicRoot : _privateRoot; 
    public async Task<StoredFile?> SaveAsync(IFormFile file, string folder, FileVisibility visibility)
    {
        var filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLowerInvariant();
        var directory = Path.Combine(RootFor(visibility), folder);
        Directory.CreateDirectory(directory);
        
        var fullPath = Path.Combine(directory, filename);
        await using var fileStream = new FileStream(fullPath, FileMode.Create,  FileAccess.Write);
        await file.CopyToAsync(fileStream);

        return new StoredFile(
            filename,
            Path.GetFileName(file.FileName),
            file.Length,
            file.ContentType
        );
    }

    public void Delete(string folder,string fileName, FileVisibility visibility)
    {
        
        
        var path = ResolveSafePath(folder, fileName, visibility); 
        if (File.Exists(path)) File.Delete(path);  
    }

    private string ResolveSafePath(string folder, string fileName, FileVisibility visibility)
    {
        var directory = Path.GetFullPath(Path.Combine(RootFor(visibility), folder));  
        var fullPath = Path.GetFullPath(Path.Combine(directory, fileName));  
  
        if (!fullPath.StartsWith(directory + Path.DirectorySeparatorChar, StringComparison.Ordinal))  
            throw new InvalidOperationException("The path is not a valid file path.");  
  
        return fullPath;  
    }

    public Task<FileDownload?> OpenRead(string folder, string fileName, FileVisibility visibility)
    {
        var safeName = Path.GetFileName(fileName);
        var fullPath = ResolveSafePath(folder, fileName, visibility);
        if(!File.Exists(fullPath)) return Task.FromResult<FileDownload?>(null);
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fullPath, out var contentType))
            contentType = "application/octet-stream";
        
        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return Task.FromResult<FileDownload?>(new FileDownload(stream, contentType, safeName));
        
    }
}