using System.Security.Claims;

namespace conscoord_api.Data.Interfaces;

public interface IRoleUtils
{
    public Task<bool> HasPerms(ClaimsPrincipal? user, string[] roles);
}
