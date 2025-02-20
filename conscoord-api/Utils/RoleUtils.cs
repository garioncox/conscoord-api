using System.Security.Claims;
using conscoord_api.Data.Interfaces;

namespace conscoord_api.Utils;
public class RoleUtils(IRoleService roleService) : IRoleUtils
{
    IRoleService _RoleService = roleService;

    public async Task<bool> HasPerms(ClaimsPrincipal? user, string[] roles)
    {
        if (user is null) { return false; }
        if (user.Identity?.IsAuthenticated == false) { return false; }

        var userEmail = user?.FindFirst(ClaimTypes.Email)?.Value;
        var userRole = await _RoleService.GetRoleByEmailAsync(userEmail ?? "");

        if (userRole is null) { return false; }

        if (roles.Length == 0) { return true; }
        if (roles.Contains(userRole.Rolename)) { return true; }

        return false;
    }
}
