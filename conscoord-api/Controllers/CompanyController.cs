using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using conscoord_api.Utils;
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

    [HttpPost("add")]
    public async Task<int> AddCompany([FromBody] CompanyRequestDTO request)
    {
        var user = HttpContext.User;
        var hasPerms = await _RoleUtils.HasPerms(user, [Role.ADMIN_ROLE]);
        if (!hasPerms) { return 0; }

        return await _CompanyService.AddCompany(request.CompanyName);
    }
}
