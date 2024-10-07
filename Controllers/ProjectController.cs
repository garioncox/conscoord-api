using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;
    public ProjectController(IProjectService service)
    {
        _projectService = service;
    }

    [HttpGet("GetProjects")]
    public async Task<List<Project>> GetProjectListAsync()
    {
        return await _projectService.GetProjectListAsync();
    }

    [HttpPost()]
    public async Task CreateProject( [FromBody] ProjectDTO projectDTO)
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

    [HttpPut("edit/{id}")]
    public async Task EditProject( [FromBody] Project project, int id)
    {
        await _projectService.EditProjectAsync(project);
    }

    [HttpDelete("delete/{id}")]
    public async Task DeleteProject(int id)
    {
        await _projectService.DeleteProjectAsync(id);
    }
}