using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace conscoord_api.Services;

public class ProjectService : IProjectService
{
    readonly PostgresContext _context;
    public ProjectService(PostgresContext context)
    {
        _context = context;
    }

    public async Task<List<Project>> GetProjectListAsync()
    {
        return await _context.Projects.ToListAsync();
    }

    public async Task CreateProject(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
    }

    public Task DeleteProjectAsync(int id)
    {
        Project? project = _context.Projects.FirstOrDefault(p => p.Id == id);
        if (project != null)
        {
            _context.Projects.Remove(project);
        }
        return _context.SaveChangesAsync();
    }

    public async Task EditProjectAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }
}