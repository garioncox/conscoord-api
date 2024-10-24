using conscoord_api.Data;
using conscoord_api.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace conscoord_api.Services;

public class RoleService : IRoleService
{
    readonly PostgresContext _context;
    public RoleService(PostgresContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetRoleByEmailAsync(string email)
    {
        return await _context.Employees
            .Where(e => e.Email == email)
            .Select(e => e.Role)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Role>> GetRoleListAsync()
    {
        return await _context.Roles.ToListAsync();
    }
}