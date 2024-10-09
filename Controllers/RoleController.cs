using conscoord_api.Data.Interfaces;
using conscoord_api.Data;
using Microsoft.AspNetCore.Mvc;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _RoleService;
    public RoleController(IRoleService service)
    {
        _RoleService = service;
    }

    [HttpGet("getAll")]
    public async Task<List<Role>> GetRoleListAsync()
    {
        return await _RoleService.GetRoleListAsync();
    }
}
