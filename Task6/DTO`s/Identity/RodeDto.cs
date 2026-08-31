using System.ComponentModel.DataAnnotations;

namespace Task6.DTO_s.Identity;

public class RoleDto
{
    [Required]
    public string RoleName { get; set; } = null!;
}

  

public class UserRoleDto //Для додавання юзеру роль
{
    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public string RoleName { get; set; } = null!;

}