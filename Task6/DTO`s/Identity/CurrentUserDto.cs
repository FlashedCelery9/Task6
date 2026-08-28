namespace Task6.DTO_s.Identity;

public class CurrentUserDto
{
    public string UserId { get; set; } = null!;
    public string Email{get;set;} = null!;
    public List<string> Roles { get; set; } = new List<string>();
    public bool IsAdmin { get; set; }

}