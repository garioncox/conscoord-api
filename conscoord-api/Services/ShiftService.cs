using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace conscoord_api.Services;

public class ShiftService : IShiftService
{
    readonly PostgresContext _context;
    public ShiftService(PostgresContext context)
    {
        _context = context;
    }

    public async Task<List<Shift>> GetShiftsWithErrorsByCompany(int companyId)
    {
        return await _context.Shifts
            .Where(s => s.ProjectShifts
                .Any(ps => ps.Project.CompanyProjects
                    .Any(cp => cp.CompanyId == companyId)) &&
                    s.EmployeeShifts.Any(es => es.ClockInTime == null || es.ClockInTime == "" ||
                                               es.ClockOutTime == null || es.ClockOutTime == ""))
            .Distinct()
            .ToListAsync();
    }

    public async Task ArchiveShiftAsync(int shift_id)
    {
        var shift = await _context.Shifts
            .Where(s => s.Id == shift_id)
            .FirstOrDefaultAsync();

        if (shift != null)
        {
            shift.Status = Shift.STATUS_ARCHIVED;
            shift.Archivedat = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            _context.Shifts.Update(shift);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> CreateShift(Shift shift)
    {
        _context.Shifts.Add(shift);
        await _context.SaveChangesAsync();
        return shift.Id;
    }

    public async Task EditShiftAsync(Shift shift)
    {
        _context.Shifts.Update(shift);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Shift>> GetAllShifts()
    {
        return await _context.Shifts
            .Where(s => s.Status == Shift.STATUS_ACTIVE)
            .ToListAsync();
    }

    public async Task<List<Shift>> GetAllArchivedAndCompletedShifts()
    {
        return await _context.Shifts
            .Where(s => s.Status != Shift.STATUS_ACTIVE)
            .ToListAsync();
    }

    public async Task<Shift?> GetShiftById(int id)
    {
        return await _context.Shifts
            .Where(s => s.Id == id)
            .FirstOrDefaultAsync();
    }

    public Task DeleteShiftAsync(int shiftId)
    {
        var shift = _context.Shifts.FirstOrDefault(s => s.Id == shiftId);
        if (shift != null)
        {
            _context.Shifts.Remove(shift);
        }
        return _context.SaveChangesAsync();
    }

    public Task<Shift[]> GetShiftByProjectAsync(Project project)
    {
        // TODO: KGB-111
        return Task.FromResult(new Shift[0]);
    }

    public List<Shift> GetScheduledShiftsByEmpId(int id)
    {
        return _context.EmployeeShifts
            .Include(e => e.Emp)
            .Where(e => e.Emp.Id == id)
            .Select(e => e.Shift)
            .ToList();
    }

    public List<Shift> GetScheduledShiftsByEmail(string email)
    {
        return _context.EmployeeShifts
            .Include(e => e.Emp)
            .Where(e => e.Emp.Email == email)
            .Select(e => e.Shift)
            .ToList();
    }

    public async Task<List<Shift>> GetShiftsByProject(int projectId)
    {
        var EmployeeShifts = await _context.ProjectShifts
            .Where(es => es.ProjectId == projectId)
            .ToListAsync();

        var shiftIds = EmployeeShifts.Select(e => e.ShiftId).ToList();

        return await _context.Shifts
            .Where(s => shiftIds.Contains(s.Id)).ToListAsync();
    }
}
