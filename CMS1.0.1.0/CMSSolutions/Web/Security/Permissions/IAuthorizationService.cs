namespace CMSSolutions.Web.Security.Permissions
{
    /// <summary>
    /// Entry-point for configured authorization scheme. Role-based system provided by default.
    /// </summary>
    public interface IAuthorizationService : IDependency
    {
        void CheckAccess(Permission permission, IUserInfo user);

        bool TryCheckAccess(Permission permission, IUserInfo user);
    }
}