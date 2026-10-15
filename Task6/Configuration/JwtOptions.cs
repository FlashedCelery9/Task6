namespace Task6.Configuration;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Key { get; set; } = null!;
    public int AccessTokenMinutes { get; set; } = 10;

}