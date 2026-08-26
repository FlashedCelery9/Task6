using System.Net.Mail;
using Task6.Models;

namespace Task6.Services;

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

    public async Task<string> SaveAsync(IFormFile file, string folder, FileVisibility visibility)
    {
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName).ToLowerInvariant(); 
        var directory = Path.Combine(RootFor(visibility), folder);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, fileName);
        await using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
        
        await file.CopyToAsync(stream);
        return fileName;
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

    public Stream? OpenRead(string folder, string fileName, FileVisibility visibility)
    {
        return null;
    }

}