using conscoord_api.Data;
using conscoord_api.Data.DTOs;
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

    public List<EmployeeShift> GetallEmployeeShifts()
    {
        return _context.EmployeeShifts.ToList();
    }

    public async Task CreateEmployeeShift(EmployeeShift empShift)
    {
        _context.EmployeeShifts.Add(empShift);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEmpShift(EmployeeShiftDTO empShift)
    {
        var newEmpShift = await _context.EmployeeShifts
            .SingleOrDefaultAsync(es => es.Id == empShift.Id);

        if (newEmpShift == null)
        {
            throw new ArgumentException("EmployeeShift not found.");
        }

        newEmpShift.ClockInTime = empShift.ClockInTime;
        newEmpShift.ClockOutTime = empShift.ClockOutTime;
        newEmpShift.DidNotWork = empShift.Didnotwork;
        newEmpShift.Notes = empShift.Notes;
        newEmpShift.ReportedCanceled = empShift.Reportedcanceled;

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
          .Where(s => s.Shift.StartTime >= currentTime)
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
                var startTime = s.Shift.StartTime;
                return startTime >= start && startTime <= end;
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

    public async Task<List<EmployeeHistoryDTO>> GetHistoryByEmail(string email)
    {
        var empshifts = await _context.EmployeeShifts
    .Include(es => es.Emp)
    .Include(es => es.Shift)
        .ThenInclude(s => s.ProjectShifts)
        .ThenInclude(ps => ps.Project)
    .Where(es => es.Emp.Email == email)
    .ToListAsync();

        if (empshifts == null || !empshifts.Any())
        {
            return new List<EmployeeHistoryDTO>();
        }

        List<EmployeeHistoryDTO> value = empshifts.Select(es =>
        {
            if (es.ClockInTime is null || es.ClockOutTime is null)
            {
                return new EmployeeHistoryDTO()
                {
                    location = es.Shift.Location ?? "",
                    hours = "--",
                    date = es.Shift.StartTime,
                    projectName = es.Shift.ProjectShifts.FirstOrDefault()?.Project.Name ?? ""
                };
            }

            var ClockOutParsed = es.ClockOutTime;
            var ClockInParsed = es.ClockInTime;
            var hoursWorked = ClockOutParsed - ClockInParsed;

            return new EmployeeHistoryDTO()
            {
                location = es.Shift.Location ?? "",
                hours = hoursWorked.Value.TotalHours.ToString(),
                date = es.Shift.StartTime,
                projectName = es.Shift.ProjectShifts.FirstOrDefault()?.Project.Name ?? ""
            };

        }).ToList();

        return value;
    }
}
