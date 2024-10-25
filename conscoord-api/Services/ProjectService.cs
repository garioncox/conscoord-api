using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;

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
        var project = _context.Projects.FirstOrDefault(p => p.Id == id);
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

    public async Task ArchiveProjectAsync(Project project)
    {
        project.Status = Shift.STATUS_ARCHIVED;
        await EditProjectAsync(project);
    }

    public async Task<List<Project>> GetCompanyProjectsAsync(Employee employee)
    {
        //we should prob find a better way to check roles as well
        if (employee.Role?.Id == 3)
        {
            return await _context.Projects
                .Include(p => p.CompanyProjects)
                .ThenInclude(cp => cp.Company)
                .ThenInclude(c => c.Employees)
                .Where(e => e.Id == employee.Id).ToListAsync();
        }

        return new List<Project>();
    }
}
