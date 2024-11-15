using conscoord_api.Data;
using conscoord_api.Data.DTOs;
using conscoord_api.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Esf;

namespace conscoord_api.Services;

public class EmployeeShiftService : IEmployeeShiftService
{
    readonly PostgresContext _context;
    public EmployeeShiftService(PostgresContext context)
    {
        _context = context;
    }

    public List<EmployeeShift> GetallEmployeeShifts()
    {
        return _context.EmployeeShifts.ToList();
    }

    public async Task CreateEmployeeShift(EmployeeShift empShift)
    {
        _context.EmployeeShifts.Add(empShift);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEmpShift(EditEmployeeShiftDTO empShift)
    {
        var newEmpShift = await _context.EmployeeShifts
            .SingleOrDefaultAsync(es => es.Id == empShift.id);

        if (newEmpShift == null)
        {
            throw new ArgumentException("EmployeeShift not found.");
        }

        newEmpShift.ClockInTime = empShift.clockInTime;
        newEmpShift.ClockOutTime = empShift.clockOutTime;

        _context.EmployeeShifts.Update(newEmpShift);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteEmpShiftAsync(int shiftId)
    {
        var shift = _context.EmployeeShifts
            .Where(s => s.ShiftId == shiftId)
            .FirstOrDefault();

        if (shift != null)
        {
            _context.EmployeeShifts.Remove(shift);
            await _context.SaveChangesAsync();
        }
    }

    public List<EmployeeShift> GetFutureShifts()
    {
        var currentTime = DateTime.Now;
        var futureShifts = _context.EmployeeShifts
          .Include(s => s.Shift)
          .Include(e => e.Emp)
          .AsEnumerable()
          .Where(s => DateTime.ParseExact(s.Shift.StartTime, "yyyy/MM/dd HH:mm:ss", null) >= currentTime)
          .ToList();

        return futureShifts;
    }

    public List<EmployeeShift> GetShiftsWithinTime(DateTime start, DateTime end)
    {
        var shifts = _context.EmployeeShifts
            .Include(s => s.Shift)
            .Include(e => e.Emp)
            .AsEnumerable()
            .Where(s =>
            {
                DateTime startTime;
                var parsed = DateTime.TryParseExact(s.Shift.StartTime, "yyyy/MM/dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out startTime);
                return parsed && startTime >= start && startTime <= end;
            })
            .ToList();

        return shifts;
    }

    public Task<List<EmployeeShift>> GetEmployeeShiftsByEmail(string email)
    {
        return _context.EmployeeShifts
            .Include(es => es.Emp)
            .Where(es => es.Emp.Email == email)
            .ToListAsync();
    }
}
