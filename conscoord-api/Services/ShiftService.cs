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

    public async Task ArchiveShiftAsync(int shift_id)
    {
        Shift? shift = await _context.Shifts
            .Where(s => s.Id == shift_id)
            .FirstOrDefaultAsync();

        if (shift != null)
        {
            shift.Status = Shift.STATUS_ARCHIVED;
            _context.Shifts.Update(shift);
            await _context.SaveChangesAsync();
        }
    }

    public async Task CreateShift(Shift shift)
    {
        _context.Shifts.Add(shift);
        await _context.SaveChangesAsync();
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
        Shift? shift = _context.Shifts.FirstOrDefault(s => s.Id == shiftId);
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
}
