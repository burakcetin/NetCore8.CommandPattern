using System.Linq;
using System.Security.Claims;

namespace NetCore8.CommandPattern.Infrastructure
{
    public class AuthorizationHandler
    {
        public bool HasPermissions(ClaimsPrincipal user, string[] requiredPermissions)
        {
            return requiredPermissions.All(permission => 
                user.Claims.Any(c => c.Type == "Permission" && c.Value == permission));
        }
    }
}