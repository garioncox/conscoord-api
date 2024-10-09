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

    public async Task<List<Role>> GetRoleListAsync()
    {
        return await _context.Roles.ToListAsync();
    }
}