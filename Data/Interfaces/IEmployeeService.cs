namespace conscoord_api.Data.Interfaces;

public interface IEmployeeService
{
    public Task ResignFromShift(int shift_id);
    public Task<List<Employee>> GetEmployeesListAsync();
    public Task<Employee> GetEmployeeByEmailAsync(string email);
    public Task<Employee?> GetEmployeeByIdAsync(int id);
    public Task PostEmployee(Employee employee);
}
