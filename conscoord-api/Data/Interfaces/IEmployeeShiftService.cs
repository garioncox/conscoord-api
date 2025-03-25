using conscoord_api.Data.DTOs;

namespace conscoord_api.Data.Interfaces;

public interface IEmployeeShiftService
{
    List<EmployeeShift> GetallEmployeeShifts();
    Task CreateEmployeeShift(EmployeeShift empShift);
    Task DeleteEmpShiftByShiftIdAsync(int shiftId);
    Task<bool> DeleteEmpShiftByShiftIdAndEmployeeIdAsync(int shiftId, int employeeId);
    List<EmployeeShift> GetFutureShifts();
    List<EmployeeShift> GetShiftsWithinTime(DateTime start, DateTime End);
    Task UpdateEmpShift(EmployeeShiftDTO empShift);
    Task<List<EmployeeShift>> GetEmployeeShiftsByEmail(string email);
    Task<List<EmployeeHistoryDTO>> GetHistoryByEmail(string email);
}
