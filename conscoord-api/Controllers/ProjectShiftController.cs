using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectShiftController : ControllerBase
{
    private readonly IProjectShiftService _projectShiftService;
    private readonly IProjectService _projectService;
    private readonly IShiftService _shiftService;
    private readonly IRoleUtils _roleUtils;
    public ProjectShiftController(IProjectShiftService projectShiftService, IProjectService projectService, IShiftService shiftService, IRoleUtils roleUtils)
    {
        _projectShiftService = projectShiftService;
        _projectService = projectService;
        _shiftService = shiftService;
        _roleUtils = roleUtils;
    }

    [HttpGet("getAll")]
    public async Task<List<ProjectShift>> GetProjectRoleListAsync()
    {
        return await _projectShiftService.GetAllProjectShifts();
    }

    [HttpPost("add")]
    public async Task<ActionResult> CreateProjectShift(ProjectShiftDTO dto)
    {
        var user = HttpContext.User;
        var hasPerms = await _roleUtils.HasPerms(user, [Role.ADMIN_ROLE, Role.CLIENT_ROLE]);
        if (!hasPerms) { return BadRequest("User is not logged in"); }

        var project = await _projectService.GetProjectByIdAsync(dto.ProjectId);
        if (project == null) { return BadRequest($"There is no project with the ID specified ({dto.ProjectId})"); }

        var shiftDTO = dto.Shift;

        if (shiftDTO.StartTime > project.EndDate ||
            shiftDTO.EndTime < project.StartDate)
        {
            return BadRequest("Shift cannot be created outside the project timeline");
        }

        Shift shift = new()
        {
            StartTime = shiftDTO.StartTime,
            EndTime = shiftDTO.EndTime,
            Description = shiftDTO.Description,
            Location = shiftDTO.Location,
            RequestedEmployees = shiftDTO.RequestedEmployees,
            Status = Shift.STATUS_ACTIVE,
        };

        var shiftId = await _shiftService.CreateShift(shift);
        await _projectShiftService.CreateProjectShiftAsync(dto.ProjectId, shiftId);
        return Ok();
    }

    [HttpDelete("delete")]
    public async Task DeleteProjectShiftAsync(int projectShiftID)
    {
        await _projectShiftService.DeleteProjectShiftAsync(projectShiftID);
    }
}
