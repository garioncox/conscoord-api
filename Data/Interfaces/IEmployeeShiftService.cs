namespace conscoord_api.Data.Interfaces;

public interface IEmployeeShiftService
{ 
    Task CreateEmployeeShift(EmployeeShift empShift);
    Task DeleteEmpShiftAsync(int shiftId);
    List<Shift> GetScheduledShiftsByEmpId(int empId);
}