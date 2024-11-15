using conscoord_api.Data.DTOs;

namespace conscoord_api.Data.Interfaces;

public interface IProjectShiftService
{
    Task CreateProjectShiftAsync(ProjectShiftDTO projectShift);
    Task DeleteProjectShiftAsync(int shiftId);
    Task<int> GetProjectShiftsForProject(int projectId);
    Task<List<ProjectShift>> GetAllProjectShifts();
}
