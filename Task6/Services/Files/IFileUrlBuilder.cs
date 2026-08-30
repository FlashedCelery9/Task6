namespace Task6.Services;

public interface IFileUrlBuilder
{
    string? PublicUrl(string? fileName, string folder);
    string? EndpointUrl(string fileName); //not working
}