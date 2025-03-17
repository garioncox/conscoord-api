using conscoord_api.Data.DTOs;

namespace conscoord_api.Data.Interfaces;

public interface IProjectService
{
    public Task<List<Project>> GetProjectListAsync();
    public Task CreateProject(Project project, int companyId);
    public Task DeleteProjectAsync(int id);
    public Task EditProjectAsync(Project project);
    public Task ArchiveProjectAsync(Project project);
    public Task<List<Project>> GetCompanyProjectsAsync(Employee employee);
    public Task<List<Project>> GetProjectArchivedAsync();
    public Task<Project?> GetProjectByIdAsync(int projectId);
    public Task<ProjectDetailsDTO> GetProjectDetailsAsync(int projectId);
}
