using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace conscoord_api.Services;

public class EmployeeService : IEmployeeService
{
    readonly PostgresContext _context;
    public EmployeeService(PostgresContext context)
    {
        _context = context;
    }

    public async Task<Employee> GetEmployeeByIdAsync(int id)
    {
        return await _context.Employees
        .Where(e => e.Id == id)
        .FirstOrDefaultAsync();
    }

    public async Task<List<Employee>> GetEmployeesListAsync()
    {
        return await _context.Employees.ToListAsync();
    }

    public async Task PostEmployee(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
    }

    public async Task ResignFromShift(int shift_id)
    {
        EmployeeShift? shift = await _context.EmployeeShifts
            .Where(es => es.ShiftId == shift_id)
            .FirstOrDefaultAsync();

        if (shift != null)
        {
            _context.EmployeeShifts.Remove(shift);
            await _context.SaveChangesAsync();
        }
    }
}
