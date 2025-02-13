using conscoord_api.Data.DTOs;

namespace conscoord_api.Data.Interfaces;

public interface IEmployeeShiftService
{
    List<EmployeeShift> GetallEmployeeShifts();
    Task CreateEmployeeShift(EmployeeShift empShift);
    Task DeleteEmpShiftAsync(int shiftId);
    List<EmployeeShift> GetFutureShifts();
    List<EmployeeShift> GetShiftsWithinTime(DateTime start, DateTime End);
    Task UpdateEmpShift(EmployeeShiftDTO empShift);
    Task<List<EmployeeShift>> GetEmployeeShiftsByEmail(string email);
    Task<List<EmployeeHistoryDTO>> GetHistoryByEmail(string email);
}
