using System.Security.Claims;
using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using conscoord_api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IShiftService _shiftService;
    private readonly IEmployeeService _employeeService;
    private readonly RoleUtils _roleUtils;
    public ProjectController(RoleUtils roleservice, IProjectService projectService, IShiftService shiftService, IEmployeeService employeeService)
    {
        _projectService = projectService;
        _shiftService = shiftService;
        _employeeService = employeeService;
        _roleUtils = roleservice;
    }

    [HttpGet("getAll/Archived")]
    public async Task<List<Project>> GetProjectArchivedAsync()
    {
        return await _projectService.GetProjectArchivedAsync();
    }

    [HttpGet("getAll")]
    public async Task<List<Project>> GetProjectListAsync()
    {
        return await _projectService.GetProjectListAsync();
    }

    [HttpPost("add")]
    public async Task CreateProject([FromBody] ProjectDTO projectDTO)
    {
        var user = HttpContext.User;
        var hasPerms = await _roleUtils.HasPerms(user, [Role.ADMIN_ROLE, Role.CLIENT_ROLE]);

        if (!hasPerms) { return; }

        var userEmail = user?.FindFirst(ClaimTypes.Email)?.Value;
        var employee = await _employeeService.GetEmployeeByEmailAsync(userEmail ?? "");

        if (employee is null || employee?.Companyid is null) { return; }

        Project project = new Project()
        {
            EndDate = projectDTO.EndDate,
            StartDate = projectDTO.StartDate,
            Location = projectDTO.Location,
            Name = projectDTO.Name,
            Status = Shift.STATUS_ACTIVE,
            Contactinfo = projectDTO.Contactinfo
        };

        await _projectService.CreateProject(project, (int) employee.Companyid);
    }

    [HttpPut("archive")]
    public async Task ArchiveProject([FromBody] Project project)
    {
        var shifts = await _shiftService.GetShiftByProjectAsync(project);

        foreach (var s in shifts)
        {
            await _shiftService.ArchiveShiftAsync(s.Id);
        }

        await _projectService.ArchiveProjectAsync(project);
    }

    [HttpPut("edit")]
    public async Task EditProject([FromBody] Project project)
    {
        await _projectService.EditProjectAsync(project);
    }

    [HttpDelete("delete/{id}")]
    public async Task DeleteProject(int id)
    {
        await _projectService.DeleteProjectAsync(id);
    }

    [HttpGet("getCompanyProjects/{empId}")]
    public async Task<ActionResult<List<Project>>> GetCompanyProjects(int empId)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(empId);
        if (employee == null)
        {
            return NotFound("Employee not found with that id");
        }
        var result = await _projectService.GetCompanyProjectsAsync(employee);

        if (result.Count == 0)
        {
            return NotFound("The employee has no associated projects");
        }
        return Ok(result);
    }
}
