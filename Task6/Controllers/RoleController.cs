using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task6.DTO_s.Identity;
using Task6.Services;

namespace Task6.Controllers;
[Authorize(Roles = "admin")]
[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly RoleService _roleService;

    public RoleController(RoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateRole(RoleDto roleDto)
    {
        var result = await _roleService.CreateRoleAsync(roleDto);
        if (result.Succeeded)
        {
            return Ok($"Role {roleDto.RoleName} created!");
        }
        return BadRequest(result.Errors);
    }

    [HttpPost("{role}delete")]
    public async Task<IActionResult> DeleteRole(string role)
    {
        var result = await _roleService.DeleteRoleAsync(role);
        if (result.Succeeded)
        {
            return Ok($"Role {role} deleted!");
        }
        return BadRequest(result.Errors);
    }

    [HttpGet("all_roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        return Ok( _roleService.GetAllRoles());
    }
    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole(UserRoleDto dto)
    {
        var result = await _roleService.AssignRoleAsync(dto);

        if (result.Succeeded)
            return Ok($"Роль '{dto.RoleName}' призначена");

        return BadRequest(result.Errors);
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveRole(UserRoleDto dto)
    {
        var result = await _roleService.RemoveRoleAsync(dto);

        if (result.Succeeded)
            return Ok($"Роль '{dto.RoleName}' забрана");

        return BadRequest(result.Errors);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var roles = await _roleService.GetUserRolesAsync(userId);
        return Ok(roles);
    }
    
    
}