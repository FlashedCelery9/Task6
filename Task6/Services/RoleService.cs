using Microsoft.AspNetCore.Identity;
using Task6.DTO_s.Identity;
using Task6.Models;

namespace Task6.Services;

public class RoleService
{
    private readonly RoleManager<IdentityRole> _roleManager; //Отримання менеджера ролей для маніпуляцій з ролями
    private readonly UserManager<AppUser> _userManager;// Для маніпуляцій з ролями в юзерів

    public RoleService(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IdentityResult> CreateRoleAsync(RoleDto roleDto)
    {
        if (await _roleManager.RoleExistsAsync(roleDto.RoleName))
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = $"Role {roleDto.RoleName} already exists"
            });
        } 
        
        return await _roleManager.CreateAsync(new IdentityRole(roleDto.RoleName));
    }

    public async Task<IdentityResult> DeleteRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description =  $"Role {roleName} does not exist"
            });
        }
        return await _roleManager.DeleteAsync(role);
    }
    public List<string> GetAllRoles()
    {
        return _roleManager.Roles.Select(r => r.Name!).ToList();
    }
    
    public async Task<IdentityResult> AssignRoleAsync(UserRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);

        if (user == null)
            return IdentityResult.Failed(new IdentityError
            {
                Description = "Користувача не знайдено"
            });

        if (!await _roleManager.RoleExistsAsync(dto.RoleName))
            return IdentityResult.Failed(new IdentityError
            {
                Description = $"Роль '{dto.RoleName}' не існує"
            });

        return await _userManager.AddToRoleAsync(user, dto.RoleName);
    }

    public async Task<IdentityResult> RemoveRoleAsync(UserRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);

        if (user == null)
            return IdentityResult.Failed(new IdentityError
            {
                Description = "Користувача не знайдено"
            });

        return await _userManager.RemoveFromRoleAsync(user, dto.RoleName);
    }

    public async Task<IList<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return new List<string>();

        return await _userManager.GetRolesAsync(user);
    }
    
}