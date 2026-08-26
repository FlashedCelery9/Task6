using Microsoft.IdentityModel.Tokens.Experimental;

namespace Task6.Validators;

public static class FileValidators
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
    private static readonly string[] AllowedMimeTypes = ["image/jpeg", "image/png", "image/webp", "application/pdf"];

    private static readonly Dictionary<string, byte[][]> Signatures = new()
    {
        [".jpeg"] = [[0xFF, 0xD8, 0xFF]],
        [".jpg"] = [[0xFF, 0xD8, 0xFF]],
        [".png"] = [[0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]],
        [".webp"] = [[0x52, 0x49, 0x46, 0x46]],
        [".pdf"] = [[0x25, 0x50, 0x44, 0x46, 0x2D]]
    };

    public static string? ValidateFile(IFormFile file, long maxBytes)
    {
        if (file is null || file.Length == 0) return "File is empty";
        if (file.Length > maxBytes) return "File is too large";

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension)) return "File extension is wrong";

        if (!AllowedMimeTypes.Contains(file.ContentType)) return "File type is wrond";

        if (!ValidateSignature(file, extension)) return "The file's content does not match its extension";
        return null;

    }

    private static bool ValidateSignature(IFormFile file, string ext)
    {
        if (!Signatures.TryGetValue(ext, out var signature)) return false;

        using var stream = file.OpenReadStream();


        Span<byte> header = stackalloc byte[8];
        var read = stream.Read(header);

        foreach (var sig in signature)
        {
            if (read >= sig.Length && header.Slice(0, sig.Length).SequenceEqual(sig))
                return true;
        }

        return false;
    }
}