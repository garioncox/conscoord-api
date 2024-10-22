namespace conscoord_api.Data.Interfaces;

public interface IEmployeeShiftService
{
    Task CreateEmployeeShift(EmployeeShift empShift);
    Task DeleteEmpShiftAsync(int shiftId);
    List<Shift> GetScheduledShiftsByEmpId(int empId);
    List<EmployeeShift> GetFutureShifts();
    List<Shift> getSignedUpShift(string email);
    List<EmployeeShift> GetShiftsWithinTime(DateTime start, DateTime End);
}