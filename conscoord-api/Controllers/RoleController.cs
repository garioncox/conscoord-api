using System.Security.Claims;
using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using conscoord_api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _RoleService;
    private readonly RoleUtils _RoleUtils;
    public RoleController(IRoleService service, RoleUtils roleUtils)
    {
        _RoleService = service;
        _RoleUtils = roleUtils;
    }

    [HttpGet("getAll")]
    public async Task<List<Role>> GetRoleListAsync()
    {
        return await _RoleService.GetRoleListAsync();
    }

    [HttpGet("get/currentUser")]
    public async Task<ActionResult<Role>> GetRoleForCurrentUserAsync()
    {
        var user = HttpContext.User;
        var hasPerms = await _RoleUtils.HasPerms(user, Role.ALL_ROLES);
        if (!hasPerms) { return NotFound(); }

        var userEmail = user?.FindFirst(ClaimTypes.Email)?.Value;
        var role = await _RoleService.GetRoleByEmailAsync(userEmail ?? "");

        if (role == null)
        {
            return NotFound();
        }

        return Ok(role);
    }

    [HttpGet("getByEmail/{email}")]
    public async Task<ActionResult<Role>> GetRoleByEmailAsync(string email)
    {
        var user = HttpContext.User;
        var hasPerms = await _RoleUtils.HasPerms(user, Role.ALL_ROLES);
        if (!hasPerms) { return NotFound(); }

        var role = await _RoleService.GetRoleByEmailAsync(email);

        if (role == null)
        {
            return NotFound();
        }

        return Ok(role);
    }
}
