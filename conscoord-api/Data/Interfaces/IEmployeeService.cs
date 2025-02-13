namespace conscoord_api.Data.Interfaces;

public interface IEmployeeService
{
    public Task ResignFromShift(int shift_id);
    public Task<List<Employee>> GetEmployeesListAsync();
    public Task<List<Employee>> GetEmployeesByShiftIdAsync(int shiftId);
    public Task<Employee?> GetEmployeeByEmailAsync(string email);
    public Task<Employee?> GetEmployeeByIdAsync(int id);
    public Task AddEmployee(Employee employee);
    public Task EditEmployee(Employee employee);
    public Task<List<Employee>> GetEmployeesSignedUpForShift(int id);
}
