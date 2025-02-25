namespace conscoord_api.Data.Interfaces;

public interface IProjectShiftService
{
    Task CreateProjectShiftAsync(int projectId, int shiftId);
    Task DeleteProjectShiftAsync(int shiftId);
    Task<List<ProjectShift>> GetAllProjectShifts();
}