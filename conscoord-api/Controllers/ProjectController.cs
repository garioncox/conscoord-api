using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace conscoord_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IShiftService _shiftService;
    private readonly IEmployeeService _employeeService;
    public ProjectController(IProjectService projectService, IShiftService shiftService, IEmployeeService employeeService)
    {
        _projectService = projectService;
        _shiftService = shiftService;
        _employeeService = employeeService;
    }

    [HttpGet("getAll")]
    public async Task<List<Project>> GetProjectListAsync()
    {
        return await _projectService.GetProjectListAsync();
    }

    [HttpPost("add")]
    public async Task CreateProject([FromBody] ProjectDTO projectDTO)
    {
        Project project = new Project()
        {
            EndDate = projectDTO.EndDate,
            StartDate = projectDTO.StartDate,
            Location = projectDTO.Location,
            Name = projectDTO.Name,
            Status = Shift.STATUS_ACTIVE
        };

        await _projectService.CreateProject(project);
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

    [HttpGet("getCompanyProducts")]
    public async Task<ActionResult<List<Project>>> GetCompanyProjects(int empId)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(empId);
        if (employee == null)
        {
            return  NotFound("Employee not found with that id");
        }
        var result = await _projectService.GetCompanyProjectsAsync(employee);

        if (result.Count == 0)
        {
            return NotFound("The employee has no associated projects");
        }
        return Ok(result);
    }
}
