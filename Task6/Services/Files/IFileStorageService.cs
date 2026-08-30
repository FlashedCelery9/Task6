namespace Task6.Services;

public enum FileVisibility
{
    Public,    // '/uploads'
    Private    // 'App_data'
}

public interface IFileStorageService
{
    Task<StoredFile?> SaveAsync(IFormFile file, string folder, FileVisibility visibility);
    void Delete(string folder, string fileName, FileVisibility visibility);
    Task<FileDownload?> OpenRead(string folder, string fileName, FileVisibility visibility);
}