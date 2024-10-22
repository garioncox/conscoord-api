namespace conscoord_api.Data.Interfaces;

public interface IRoleService
{
    public Task<List<Role>> GetRoleListAsync();
    public Task<Role?> GetRoleByEmailAsync(string email);
}
