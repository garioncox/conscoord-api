﻿using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace conscoord_api.Services;

public class EmployeeShiftService : IEmployeeShiftService
{
    readonly PostgresContext _context;
    public EmployeeShiftService(PostgresContext context)
    {
        _context = context;
    }

    public async Task CreateEmployeeShift(EmployeeShift empShift)
    {
        _context.EmployeeShifts.Add(empShift);
        await _context.SaveChangesAsync();
    }

    public List<Shift> GetScheduledShiftsByEmpId(int empId)
    {
        return _context.Shifts
            .Where(s => s.EmployeeShifts.Any(es => es.EmpId == empId))
            .ToList();
    }

    public async Task DeleteEmpShiftAsync(int shiftId)
    {
        EmployeeShift? shift = _context.EmployeeShifts
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
        DateTime currentTime = DateTime.Now;
        var futureShifts = _context.EmployeeShifts
          .Include(s => s.Shift)
          .Include(e => e.Emp)
          .AsEnumerable()
          .Where(s => DateTime.ParseExact(s.Shift.StartTime, "yyyy/MM/dd HH:mm:ss", null) >= currentTime)
          .ToList();

        return futureShifts;
    }

    public List<Shift> getSignedUpShift(string email)
    {
        return _context.EmployeeShifts
            .Include(e => e.Emp)
            .Where(e => e.Emp.Email == email)
            .Select(e => e.Shift)
            .ToList();
    }
}
