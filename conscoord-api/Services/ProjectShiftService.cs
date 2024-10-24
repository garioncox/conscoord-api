using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace conscoord_api.Services;

public class ProjectShiftService : IProjectShiftService
{
    readonly PostgresContext _context;
    public ProjectShiftService(PostgresContext context)
    {
        _context = context;
    }
    public async Task<List<ProjectShift>> GetAllProjectShifts()
    {
        return await _context.ProjectShifts.ToListAsync();
    }

    public async Task CreateProjectShiftAsync(ProjectShiftDTO projectShift)
    {
        var addProjectShift = new ProjectShift
        {
            ProjectId = projectShift.ProjectId,
            ShiftId = projectShift.ShiftId
        };

        _context.ProjectShifts.Add(addProjectShift);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProjectShiftAsync(int projectShiftID)
    {
        var projectShift = _context.ProjectShifts
            .Where(s => s.Id == projectShiftID)
            .FirstOrDefault();

        if (projectShift != null)
        {
            _context.ProjectShifts.Remove(projectShift);
            await _context.SaveChangesAsync();
        }
    }

}
