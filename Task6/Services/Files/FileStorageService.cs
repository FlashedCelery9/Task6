using System.Net.Mail;
using Microsoft.AspNetCore.StaticFiles;
using Task6.Models;

namespace Task6.Services;
public class StoredFile
{
    public string FileName { get; set; } = null!;
    public string OriginalFileName { get; set; } = null!;
    public long Size { get; set; }
    public string ContentType { get; set; } = null!;

    public StoredFile(string fileName, string originalFileName, long size, string contentType)
    {
        FileName = fileName;
        OriginalFileName = originalFileName;
        Size = size;
        ContentType = contentType;
    }
}

public class FileStorageService : IFileStorageService
{
    private readonly string _publicRoot;
    private readonly string _privateRoot;
    
    public FileStorageService(IWebHostEnvironment env)
    {
        _publicRoot = Path.Combine(env.ContentRootPath, "uploads");
        _privateRoot = Path.Combine(env.ContentRootPath, "App_Data", "uploads");
    }
    private string RootFor(FileVisibility v) => v == FileVisibility.Public ? _publicRoot : _privateRoot;

    public async Task<StoredFile?> SaveAsync(IFormFile file, string folder, FileVisibility visibility)
    {
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLowerInvariant();
        var directory = Path.Combine(RootFor(visibility), folder);
        Directory.CreateDirectory(directory);

        var fullPath = Path.Combine(directory, fileName);

        await using var stream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);
        await file.CopyToAsync(stream);

        return new StoredFile(
            fileName,
            Path.GetFileName(file.FileName),
            file.Length,
            file.ContentType
        );
    }
    public void Delete(string folder, string fileName, FileVisibility visibility)  
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
        var fullPath = Path.Combine(RootFor(visibility), folder, safeName);

        if (!File.Exists(fullPath)) return Task.FromResult<FileDownload?>(null);

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fullPath, out var contentType))
            contentType = "application/octet-stream";
        
        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return Task.FromResult<FileDownload?>(new FileDownload(stream, contentType, safeName));
       
    }

}