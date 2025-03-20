using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _CompanyService;
    private readonly IRoleUtils _RoleUtils;
    public CompanyController(ICompanyService service, IRoleUtils roleUtils)
    {
        _RoleUtils = roleUtils;
        _CompanyService = service;
    }

    [HttpGet("getAll")]
    public async Task<List<Company>> GetCompanyListAsync()
    {
        return await _CompanyService.GetCompanyListAsync();
    }

    [HttpGet("get/name/{projectId}")]
    public async Task<ActionResult<string>> GetCompanyNameByProjectIdAsync(int projectId)
    {
        var user = HttpContext.User;
        var hasPerms = await _RoleUtils.HasPerms(user, Role.ALL_ROLES);
        if (!hasPerms) { return BadRequest("User is not logged in"); }

        var name = await _CompanyService.GetCompanyNameByProjectIdAsync(projectId);
        if (name == null)
        {
            return BadRequest("Company not found for the project specifed");
        }

        return Ok(name);
    }

    [HttpPost("add")]
    public async Task<int> AddCompany([FromBody] CompanyRequestDTO request)
    {
        var user = HttpContext.User;
        var hasPerms = await _RoleUtils.HasPerms(user, [Role.ADMIN_ROLE]);
        if (!hasPerms) { return 0; }

        return await _CompanyService.AddCompany(request.CompanyName);
    }
}
