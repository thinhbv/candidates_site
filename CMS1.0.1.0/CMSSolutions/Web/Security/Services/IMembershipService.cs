using System;
using System.Collections.Generic;
using CMSSolutions.Services;
using CMSSolutions.Web.Security.Domain;

namespace CMSSolutions.Web.Security.Services
{
    public interface IMembershipService : IGenericService<User, int>, IDependency
    {
        #region Users

        void Login(User user, bool rememberMe = false);

        bool Login(string userName, string password, bool rememberMe = false);

        void Logout();

        bool ChangePassword(string username, string oldPassword, string newPassword);

        bool SetPassword(User user, string newPassword);

        bool ConfirmAccount(string userName, string accountConfirmationToken);

        bool ConfirmAccount(string accountConfirmationToken);

        LocalAccount CreateLocalAccount(int id, string password, bool requireConfirmationToken);

        User CreateUserAndLocalAccount(string username, string fullName, string phoneNumber, string email, string password, bool requireConfirmation);

        void CreateUserAndLocalAccount(User user, string password, bool requireConfirmation);

        bool DeleteUser(int userId);

        string GeneratePasswordResetToken(User user, int tokenExpirationInMinutesFromNow);

        User GetUser(string userName);

        User GetUser(int userId);

        User GetUserByEmail(string email);

        IEnumerable<User> GetUsers(IEnumerable<int> userIds);

		IEnumerable<User> GetUsersByRole(int roleId);

        LocalAccount GetLocalAccount(int userId);

        LocalAccount GetLocalAccount(string token);

        User ResetPasswordWithToken(string token, string newPassword);

        void LockUser(int userId);

        void UnlockUser(int userId);

        bool ValidateUser(string username, string password, out User user, bool throwException = false);

        void UpdatePasswordUser(int userId);

        #endregion Users

        bool IsUserInRole(int userId, string roleName);

        #region Roles

        Role GetRole(int id);

        Role GetRole(string name);

        Role CreateRole(string name);

        void AssignUserToRoles(int userId, params int[] roleIds);

        void AssignUserToRoles(int userId, params string[] roles);

        IEnumerable<Role> GetAllRoles();

        bool HaveUsersInRole(int roleId);

        IEnumerable<Role> GetRolesForUser(int userId);

        #endregion Roles

        #region Permissions

        IEnumerable<Permission> GetPermissionsForRole(int roleId);

        IEnumerable<Permission> GetPermissionsForRole(string roleName);

        void AssignPermissionsToRole(int roleId, params int[] permissions);

        void AssignPermissionsToRole(int roleId, params Permissions.Permission[] permissions);

        #endregion Permissions

        #region Profile

        IDictionary<string, string> GetProfile(int userId);

        void UpdateProfile(int userId, IDictionary<string, string> profile, bool deleteExisting = false);

        string GetProfileEntry(int userId, string key);

        void SaveProfileEntry(int userId, string key, string value);

        void DeleteProfileEntry(int userId, string key);

        IEnumerable<UserProfileEntry> GetProfileEntriesByKey(string key);

        List<User> SearchUserPaged(
            string searchText,
            int pageIndex,
            int pageSize,
            out int totalRecord);

        List<User> SearchUserByRolePaged(
            string searchText,
            int roleId,
            int pageIndex,
            int pageSize,
            out int totalRecord);

        #endregion
    }
}