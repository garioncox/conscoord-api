using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _CompanyService;
    public CompanyController(ICompanyService service)
    {
        _CompanyService = service;
    }

    [HttpGet("getAll")]
    public async Task<List<Company>> GetCompanyListAsync()
    {
        return await _CompanyService.GetCompanyListAsync();
    }
}
