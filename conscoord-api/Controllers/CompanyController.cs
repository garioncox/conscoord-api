using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using conscoord_api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _CompanyService;
    private readonly RoleUtils _RoleUtils;
    public CompanyController(ICompanyService service, RoleUtils roleUtils)
    {
        _RoleUtils = roleUtils;
        _CompanyService = service;
    }

    [HttpGet("getAll")]
    public async Task<List<Company>> GetCompanyListAsync()
    {
        return await _CompanyService.GetCompanyListAsync();
    }

    public class AddCompanyRequest
    {
        public string CompanyName { get; set; }
    }

    [HttpPost("add")]
    public async Task AddCompany([FromBody] AddCompanyRequest request)
    {
        var user = HttpContext.User;
        var hasPerms = await _RoleUtils.HasPerms(user, [Role.ADMIN_ROLE]);
        if (!hasPerms) { return; }

        await _CompanyService.AddCompany(request.CompanyName);
    }
}
