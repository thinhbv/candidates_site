using System.Collections.Generic;

namespace CMSSolutions.Web.Security.Permissions
{
    /// <summary>
    /// Implemented by modules to enumerate the types of permissions the which may be granted
    /// </summary>
    public interface IPermissionProvider : IDependency
    {
        IEnumerable<Permission> GetPermissions();
    }
}