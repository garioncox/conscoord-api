using conscoord_api.Data.Interfaces;
using conscoord_api.Data;
using Microsoft.AspNetCore.Mvc;
using conscoord_api.Services;
using conscoord_api.Data.DTOs;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectShiftController : ControllerBase
{
    private readonly IProjectShiftService _ProjectShiftService;
    public ProjectShiftController(IProjectShiftService service)
    {
        _ProjectShiftService = service;
    }

    [HttpGet("getAll")]
    public async Task<List<ProjectShift>> GetProjectRoleListAsync()
    {
        return await _ProjectShiftService.GetAllProjectShifts();
    }

    [HttpPost("add")]
    public async Task CreateProjectShift(ProjectShiftDTO projectShift)
    {
        await _ProjectShiftService.CreateProjectShiftAsync(projectShift);
    }

    [HttpDelete("delete")]
    public async Task DeleteProjectShiftAsync(int projectShiftID)
    {
        await _ProjectShiftService.DeleteProjectShiftAsync(projectShiftID);
    }
}