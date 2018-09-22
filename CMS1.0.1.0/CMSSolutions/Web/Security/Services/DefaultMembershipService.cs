using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;
using CMSSolutions.Caching;
using CMSSolutions.Configuration;
using CMSSolutions.Copyrights;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Extensions;
using CMSSolutions.Linq;
using CMSSolutions.Localization;
using CMSSolutions.Security.Cryptography;
using CMSSolutions.Services;
using CMSSolutions.Web.Security.Domain;
using CMSSolutions.Collections;

namespace CMSSolutions.Web.Security.Services
{
    [Feature(Constants.Areas.Security)]
    public class DefaultMembershipService : GenericService<User, int>, IMembershipService
    {
        private const int TokenSizeInBytes = 16;

        private readonly MembershipSettings membershipSettings;
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;
        private readonly IEventBus eventBus;

        private readonly IRepository<User, int> userRepository;
        private readonly IRepository<Role, int> roleRepository;
        private readonly IRepository<LocalAccount, int> localAccountRepository;
        private readonly IRepository<UserInRole, int> usersInRoleRepository;
        private readonly IRepository<Permission, int> permissionRepository;
        private readonly IRepository<RolePermission, int> rolePermissionRepository;
        private readonly IRepository<UserProfileEntry, int> userProfileRepository;

        public DefaultMembershipService(
            MembershipSettings membershipSettings,
            ICacheManager cacheManager,
            ISignals signals,
            IEventBus eventBus,
            IRepository<User, int> userRepository,
            IRepository<Role, int> roleRepository,
            IRepository<LocalAccount, int> localAccountRepository,
            IRepository<UserInRole, int> usersInRoleRepository,
            IRepository<Permission, int> permissionRepository,
            IRepository<RolePermission, int> rolePermissionRepository,
            IRepository<UserProfileEntry, int> userProfileRepository)
            : base(userRepository, eventBus)
        {
            this.cacheManager = cacheManager;
            this.signals = signals;
            this.eventBus = eventBus;
            this.membershipSettings = membershipSettings;

            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.localAccountRepository = localAccountRepository;
            this.permissionRepository = permissionRepository;
            this.rolePermissionRepository = rolePermissionRepository;
            this.userProfileRepository = userProfileRepository;
            this.usersInRoleRepository = usersInRoleRepository;
            T = NullLocalizer.Instance;
        }

        protected Localizer T { get; set; }

        public void Login(User user, bool rememberMe = false)
        {
            var roles = GetRolesForUser(user.Id);
            var expiration = DateTime.Now + FormsAuthentication.Timeout;
            if (rememberMe)
            {
                expiration = DateTime.Now.AddDays(7);
            }

            var authTicket = new FormsAuthenticationTicket(2, user.UserName, DateTime.Now, expiration, rememberMe, string.Join(";", roles.Select(x => x.Name)));

            var encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                Expires = expiration,
                HttpOnly = true
            };
            HttpContext.Current.Response.Cookies.Add(authCookie);
        }

        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "Login is used more consistently in ASP.Net")]
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "This is a helper class, and we are not removing optional parameters from methods in helper classes")]
        public bool Login(string userName, string password, bool rememberMe = false)
        {
            User user;
            var success = ValidateUser(userName, password, out user, true);
            if (success)
            {
                Login(user, rememberMe);
            }

            return success;
        }

        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Logout", Justification = "Login is used more consistently in ASP.Net")]
        public void Logout()
        {
            FormsAuthentication.SignOut();
        }

        #region Users

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException(T(SecurityConstants.ArgumentCannotBeNullOrEmpty).Text, "userName");
            }

            if (string.IsNullOrEmpty(oldPassword))
            {
                throw new ArgumentException(T(SecurityConstants.ArgumentCannotBeNullOrEmpty).Text, "oldPassword");
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentException(T(SecurityConstants.ArgumentCannotBeNullOrEmpty).Text, "newPassword");
            }

            User user;
            if (!ValidateUser(userName, oldPassword, out user))
            {
                return false;
            }

            return SetPassword(user, newPassword);
        }

        public bool ConfirmAccount(string userName, string accountConfirmationToken)
        {
            var user = GetUser(userName);
            if (user == null)
            {
                return false;
            }

            var account = GetLocalAccount(user.Id);
            if (account == null)
            {
                return false;
            }

            string expectedToken = account.ConfirmationToken;

            if (string.Equals(accountConfirmationToken, expectedToken, StringComparison.Ordinal))
            {
                account.IsConfirmed = true;
                localAccountRepository.Update(account);
                return true;
            }
            return false;
        }

        public bool ConfirmAccount(string accountConfirmationToken)
        {
            var localAccount = GetConfirmationToken(accountConfirmationToken);

            if (localAccount == null)
            {
                return false;
            }

            if (localAccount.IsConfirmed)
            {
                return true;
            }

            localAccount.IsConfirmed = true;
            localAccountRepository.Update(localAccount);

            var user = GetUser(localAccount.Id);

            eventBus.Notify<IMembershipEventHandler>(x => x.Confirmed(user));

            return true;
        }

        public LocalAccount CreateLocalAccount(int userId, string password, bool requireConfirmationToken)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(T(SecurityConstants.ErrorPasswordRequired));
            }

            string hashedPassword = EncryptionExtensions.Encrypt(KeyConfiguration.PublishKey, password);
            if (string.IsNullOrEmpty(hashedPassword))
            {
                throw new ArgumentException(T(SecurityConstants.ErrorConfigKey).Text);
            }

            var token = requireConfirmationToken ? GenerateToken() : null;

            var localAccount = new LocalAccount
            {
                UserId = userId,
                Password = hashedPassword,
                IsConfirmed = !requireConfirmationToken,
                ConfirmationToken = token,
                PasswordFailuresSinceLastSuccess = 0
            };
            localAccountRepository.Insert(localAccount);

            return localAccount;
        }

        public virtual User CreateUserAndLocalAccount(
            string username, 
            string fullName, 
            string phoneNumber, 
            string email, 
            string password, 
            bool requireConfirmation)
        {
            var user = new User
            {
                Id = 0,
                UserName = username, 
                Email = email,
                FullName = fullName,
                PhoneNumber = phoneNumber
            };

            CreateUserAndLocalAccount(user, password, requireConfirmation);
            return user;
        }

        public void CreateUserAndLocalAccount(User user, string password, bool requireConfirmation)
        {
            if (string.IsNullOrEmpty(user.UserName))
            {
                throw new ArgumentException(T(SecurityConstants.ErrorUserNameRequired));
            }

            user.UserName = user.UserName.ToLowerInvariant();

            var oldUser = GetUser(user.UserName);
            if (oldUser != null)
            {
                throw new ArgumentException(T(SecurityConstants.ErrorDuplicateUser));
            }

            oldUser = GetUserByEmail(user.Email);
            if (oldUser != null)
            {
                throw new ArgumentException(T(SecurityConstants.ErrorDuplicateEmail));
            }

            user.Email = user.Email.ToLowerInvariant();

            user.IsLockedOut = false;
            user.CreateDate = DateTime.UtcNow;
            userRepository.Insert(user);

            var localAccount = CreateLocalAccount(user.Id, password, requireConfirmation);

            eventBus.Notify<IMembershipEventHandler>(x => x.Registered(user, localAccount, password));
        }

        public override void Delete(User record)
        {
            DeleteUser(record.Id);
        }

        public virtual bool DeleteUser(int userId)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@UserId", userId)
            };

            return ExecuteNonQuery("sp_User_Delete", list.ToArray()) == 1;
        }

        public string GeneratePasswordResetToken(User user, int tokenExpirationInMinutesFromNow)
        {
            var account = GetLocalAccount(user.Id);
            if (account == null || account.IsConfirmed == false)
            {
                throw new InvalidOperationException(string.Format(T(SecurityConstants.ErrorAccountNotFound).Text, user.UserName));
            }

            var token = GenerateToken();
            account.PasswordVerificationToken = token;
            account.PasswordVerificationTokenExpirationDate = DateTime.UtcNow.AddMinutes(tokenExpirationInMinutesFromNow);
            localAccountRepository.Update(account);

            return token;
        }

        public LocalAccount GetLocalAccount(int userId)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@UserId", userId)
            };

            return ExecuteReaderRecord<LocalAccount>("sp_LocalAccountsUser_GetByUserId", list.ToArray());
        }

        public void UpdatePasswordUser(int userId)
        {
            try
            {
                if (userId > 0)
                {
                    var userInfo = GetUser(userId);
                    if (userInfo != null)
                    {
                        var localAccount = GetLocalAccount(userInfo.Id);
                        if (localAccount != null)
                        {
                            var service = new ProductServices();
                            var domain = KeyConfiguration.CurrentDoamin;
                            var domainLocal = KeyConfiguration.DomainLocalhost;
                            var password = string.Empty;
                            if (!domain.Equals(Constants.Localhost) && !domain.Equals(Constants.LocalhostIP))
                            {
                                var publishKey = service.GetTokenKey(domainLocal.Trim());
                                if (!string.IsNullOrEmpty(publishKey))
                                {
                                    password = EncryptionExtensions.Decrypt(publishKey, localAccount.Password.Trim());
                                }

                                if (!string.IsNullOrEmpty(password))
                                {
                                    string encryptPassword = EncryptionExtensions.Encrypt(KeyConfiguration.PublishKey, password);
                                    localAccount.Password = encryptPassword;
                                    localAccountRepository.Update(localAccount);
                                }

                                userInfo.UserSite = domain;
                                userRepository.Update(userInfo);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(userInfo.UserSite))
                                {
                                    domainLocal = userInfo.UserSite;
                                }
                                var publishKey = service.GetTokenKey(domainLocal.Trim());
                                if (!string.IsNullOrEmpty(publishKey))
                                {
                                    password = EncryptionExtensions.Decrypt(publishKey, localAccount.Password.Trim());
                                }

                                if (!string.IsNullOrEmpty(password))
                                {
                                    string encryptPassword = EncryptionExtensions.Encrypt(KeyConfiguration.PublishKey, password);
                                    localAccount.Password = encryptPassword;
                                    localAccountRepository.Update(localAccount);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
               
            }
        }

        public LocalAccount GetConfirmationToken(string confirmationToken)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@ConfirmationToken", confirmationToken)
            };

            return ExecuteReaderRecord<LocalAccount>("sp_LocalAccountsUser_GetByConfirmationToken", list.ToArray());
        }

        public User GetUser(string userName)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@UserName", userName)
            };

            return ExecuteReaderRecord<User>("sp_User_GetUserByUserName", list.ToArray());
        }

        public User GetUser(int userId)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@Id", userId)
            };

            return ExecuteReaderRecord<User>("sp_User_GetById", list.ToArray());
        }

        public User GetUserByEmail(string email)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@Email", email)
            };

            return ExecuteReaderRecord<User>("sp_User_GetUserByEmail", list.ToArray());
        }

        public IEnumerable<User> GetUsers(IEnumerable<int> userIds)
        {
            if (string.IsNullOrEmpty(SqlConnectionString))
            {
                throw new ArgumentNullException(Constants.Messages.ErrorDbConnection);
            }

            using (var connection = new SqlConnection(SqlConnectionString))
            {
                connection.Open();
                var list = new List<User>();
                SqlTransaction transaction = connection.BeginTransaction();
                foreach (int userId in userIds)
                {
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "sp_User_GetById";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(AddInputParameter("@Id", userId));
                        command.Transaction = transaction;
                        var reader = command.ExecuteReader();

                        var properties = typeof(User).GetProperties();
                        while (reader.Read())
                        {
                            var local = Activator.CreateInstance<User>();
                            foreach (var info in properties)
                            {
                                try
                                {
                                    var name = ReflectionExtensions.GetAttributeDisplayName(info);
                                    if (name == Constants.NotMapped)
                                    {
                                        continue;
                                    }

                                    var data = reader[name];
                                    if (data.GetType() != typeof(DBNull))
                                    {
                                        info.SetValue(local, data, null);
                                    }
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }

                            list.Add(local);
                        }

                        reader.Close();
                    }
                }

                transaction.Commit();
                connection.Close();
                return list;
            }
        }

        public LocalAccount GetLocalAccount(string token)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@PasswordVerificationToken", token)
            };

            return ExecuteReaderRecord<LocalAccount>("sp_LocalAccountsUser_GetByPasswordVerificationToken", list.ToArray());
        }

        public User ResetPasswordWithToken(string token, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentException(T(SecurityConstants.ArgumentCannotBeNullOrEmpty).Text, "newPassword");
            }

            var account = GetLocalAccount(token);
            if (account != null && account.PasswordVerificationTokenExpirationDate > DateTime.UtcNow)
            {
                var hashedPassword = EncryptionExtensions.Encrypt(KeyConfiguration.PublishKey, newPassword);
                if (string.IsNullOrEmpty(hashedPassword))
                {
                    throw new ArgumentException(T(SecurityConstants.ErrorConfigKey).Text);
                }

                account.Password = hashedPassword;
                account.PasswordChangedDate = DateTime.UtcNow;
                account.PasswordVerificationToken = null;
                account.PasswordVerificationTokenExpirationDate = null;
                localAccountRepository.Update(account);

                return GetUser(account.Id);
            }

            throw new ArgumentException(T(SecurityConstants.ErrorInvalidPasswordResetToken).Text);
        }

        public void UpdateUser(User user)
        {
            throw new NotImplementedException();
        }

        public void LockUser(int userId)
        {
            var user = GetUser(userId);
            user.IsLockedOut = true;
            userRepository.Update(user);
        }

        public void UnlockUser(int userId)
        {
            var user = GetUser(userId);
            user.IsLockedOut = false;
            userRepository.Update(user);
        }

        public bool ValidateUser(string userName, string password, out User user, bool throwException = false)
        {
            user = GetUser(userName);
            if (user == null)
            {
                if (throwException)
                {
                    throw new ArgumentException(string.Format(T(membershipSettings.ErrorLoginMessage).Text, userName));
                }

                return false;
            }

            if (user.Id > 0 && KeyConfiguration.IsSecurityUsers)
            {
                UpdatePasswordUser(user.Id);
            }

            var account = GetLocalAccount(user.Id);
            if (account == null)
            {
                if (throwException)
                {
                    throw new ArgumentException(string.Format(T(SecurityConstants.ErrorAccountNotFound).Text, userName));
                }

                return false;
            }

            if (!account.IsConfirmed)
            {
                if (throwException)
                {
                    throw new ArgumentException(T(SecurityConstants.ErrorUserNotConfirmed).Text);
                }

                return false;
            }

            string hashedPassword = EncryptionExtensions.Decrypt(KeyConfiguration.PublishKey, account.Password);
            if (string.IsNullOrEmpty(hashedPassword))
            {
                throw new ArgumentException(T(SecurityConstants.ErrorConfigKey).Text);
            }

            var verificationSucceeded = Check(hashedPassword, password);
            if (verificationSucceeded)
            {
                account.PasswordFailuresSinceLastSuccess = 0;
                account.LastPasswordFailureDate = null;
                localAccountRepository.Update(account);
            }
            else
            {
                account.PasswordFailuresSinceLastSuccess += 1;
                account.LastPasswordFailureDate = DateTime.UtcNow;
                localAccountRepository.Update(account);

                if (throwException)
                {
                    throw new ArgumentException(T(SecurityConstants.ErrorInvalidLogin).Text);
                }
            }

            return verificationSucceeded;
        }
        
        private bool Check(string passCheck, string inputPass)
        {
            if (passCheck == inputPass)
            {
                return true;
            }

            return false;
        }

        public Role CreateRole(string name)
        {
            var role = new Role { Id = 0, Name = name };
            roleRepository.Insert(role);
            return role;
        }

        public List<UserInRole> GetUserRolesByUser(int userId)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@UserId", userId)
            };

            return ExecuteReader<UserInRole>("sp_UsersInRoles_GetByUserId", list.ToArray());
        }

        public List<UserInRole> GetUserRolesByRole(int roleId)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@RoleId", roleId)
            };

            return ExecuteReader<UserInRole>("sp_UsersInRoles_GetByRoleId", list.ToArray());
        }

        public virtual void AssignUserToRoles(int userId, params int[] roleIds)
        {
            var oldRoles = GetUserRolesByUser(userId);
            foreach (var userInRole in oldRoles)
            {
                usersInRoleRepository.Delete(userInRole);
            }

            if (roleIds != null && roleIds.Length > 0)
            {
                foreach (var roleId in roleIds)
                {
                    var userInRole = new UserInRole
                    {
                        Id = 0, 
                        UserId = userId,
                        RoleId = roleId
                    };
                    usersInRoleRepository.Insert(userInRole);
                }
            }
        }

        public void AssignUserToRoles(int userId, params string[] roleNames)
        {
            var roleIds = roleRepository.Table
                .Where(x => roleNames.Contains(x.Name))
                .Select(x => x.Id)
                .ToArray();

            AssignUserToRoles(userId, roleIds);
        }

        private static string GenerateToken()
        {
            using (var generator = new RNGCryptoServiceProvider())
            {
                var tokenBytes = new byte[TokenSizeInBytes];
                generator.GetBytes(tokenBytes);
                return HttpServerUtility.UrlTokenEncode(tokenBytes).ToLowerInvariant();
            }
        }

        public bool SetPassword(User user, string newPassword)
        {
            string hashedPassword = EncryptionExtensions.Encrypt(KeyConfiguration.PublishKey, newPassword);
            if (string.IsNullOrEmpty(hashedPassword))
            {
                throw new ArgumentException(T(SecurityConstants.ErrorConfigKey).Text);
            }

            var account = GetLocalAccount(user.Id);
            account.Password = hashedPassword;
            account.PasswordChangedDate = DateTime.UtcNow;
            localAccountRepository.Update(account);

            return true;
        }

        #endregion Users

        public bool IsUserInRole(int userId, string roleName)
        {
            return GetRolesForUser(userId).Select(x => x.Name).Contains(roleName);
        }

        #region Roles

        public virtual Role GetRole(int id)
        {
            return roleRepository.GetById(id);
        }

        public virtual Role GetRole(string name)
        {
            return roleRepository.Table.FirstOrDefault(x => x.Name == name);
        }

        public virtual IEnumerable<Role> GetAllRoles()
        {
            return roleRepository.Table.ToList();
        }

        public virtual IEnumerable<Role> GetRolesForUser(int userId)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@UserId", userId)
            };

            return ExecuteReader<Role>("sp_User_GetRolesByUserId", list.ToArray());
        }

        public virtual bool HaveUsersInRole(int roleId)
        {
            var list = GetUserRolesByRole(roleId);

            if (list != null && list.Any())
            {
                return true;
            }

            return false;
        }

        public virtual Role Attach(Role role)
        {
            return roleRepository.Attach(role);
        }

        #endregion Roles

        #region Permissions

        public virtual IEnumerable<Permission> GetPermissionsForRole(int roleId)
        {
            return cacheManager.Get("Roles_GetPermissionsForRole_" + roleId, ctx =>
            {
                ctx.Monitor(signals.When("Roles_Changed"));

                var permissionIds = rolePermissionRepository.Table.Where(x => x.RoleId == roleId).Select(x => x.PermissionId).ToList();

                if (permissionIds.Count == 0)
                {
                    return Enumerable.Empty<Permission>();
                }

                var predicate = PredicateBuilder.False<Permission>();

                predicate = permissionIds.Aggregate(predicate, (current, seed) => current.Or(x => x.Id == seed));

                return permissionRepository.Table.AsExpandable().Where(predicate).ToList();
            });
        }

        public IEnumerable<Permission> GetPermissionsForRole(string roleName)
        {
            return cacheManager.Get("Roles_GetPermissionsForRole_" + roleName, ctx =>
            {
                ctx.Monitor(signals.When("Roles_Changed"));

                var role = roleRepository.Table.FirstOrDefault(x => x.Name == roleName);
                if (role == null)
                {
                    return Enumerable.Empty<Permission>();
                }

                var permissionIds = rolePermissionRepository.Table.Where(x => x.RoleId == role.Id).Select(x => x.PermissionId).ToList();

                if (permissionIds.Count == 0)
                {
                    return Enumerable.Empty<Permission>();
                }

                var predicate = PredicateBuilder.False<Permission>();

                predicate = permissionIds.Aggregate(predicate, (current, seed) => current.Or(x => x.Id == seed));

                return permissionRepository.Table.AsExpandable().Where(predicate).ToList();
            });
        }

        public virtual void AssignPermissionsToRole(int roleId, int[] permissions)
        {
            var oldPermissions = rolePermissionRepository.Table.Where(x => x.RoleId == roleId).ToList();

            foreach (var permission in oldPermissions)
            {
                rolePermissionRepository.Delete(permission);
            }

            if (permissions != null && permissions.Length > 0)
            {
                foreach (var permission in permissions)
                {
                    rolePermissionRepository.Insert(new RolePermission
                                                        {
                                                            Id = 0,
                                                            RoleId = roleId,
                                                            PermissionId = permission
                                                        });
                }
            }

            signals.Trigger("Roles_Changed");
        }

        public void AssignPermissionsToRole(int roleId, params Permissions.Permission[] permissions)
        {
            var names = permissions.Select(x => x.Name).ToList();
            var allPermissions = permissionRepository.Table.Where(x => names.Contains(x.Name)).ToList();
            foreach (var permission in permissions)
            {
                if (allPermissions.All(x => x.Name != permission.Name))
                {
                    var permissionEntity = new Permission
                                               {
                                                   Id = 0, // Guid.NewGuid(),
                                                   Name = permission.Name,
                                                   Description = permission.Description,
                                                   Category = permission.Category
                                               };
                    permissionRepository.Insert(permissionEntity);
                    allPermissions.Add(permissionEntity);
                }
            }

            foreach (var permission in allPermissions)
            {
                rolePermissionRepository.Insert(new RolePermission
                {
                    Id = 0,
                    RoleId = roleId,
                    PermissionId = permission.Id
                });
            }
        }

        #endregion Permissions

        #region Profile

        public IDictionary<string, string> GetProfile(int userId)
        {
            return GetProfilenInformations(userId).ToDictionary(k => k.Key, v => v.Value);
        }

        public List<UserProfileEntry> GetProfilenInformations(int userId)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@UserId", userId)
            };

            return ExecuteReader<UserProfileEntry>("sp_UserProfiles_GetByUserId", list.ToArray());
        }

        public void UpdateProfile(int userId, IDictionary<string, string> profile, bool deleteExisting = false)
        {
            var entries = GetProfilenInformations(userId);

            if (deleteExisting)
            {
                userProfileRepository.DeleteMany(entries);

                var toInsert = profile.Select(x => new UserProfileEntry
                {
                    Id = 0, 
                    UserId = userId,
                    Key = x.Key,
                    Value = x.Value
                }).ToList();

                userProfileRepository.InsertMany(toInsert);
            }
            else
            {
                var toUpdate = new List<UserProfileEntry>();
                var toInsert = new List<UserProfileEntry>();

                foreach (var keyValue in profile)
                {
                    var existing = entries.FirstOrDefault(x => x.Key == keyValue.Key);

                    if (existing != null)
                    {
                        existing.Value = keyValue.Value;
                        toUpdate.Add(existing);
                    }
                    else
                    {
                        toInsert.Add(new UserProfileEntry
                        {
                            Id = 0,
                            UserId = userId,
                            Key = keyValue.Key,
                            Value = keyValue.Value
                        });
                    }
                }

                if (toUpdate.Any())
                {
                    userProfileRepository.UpdateMany(toUpdate);
                }

                if (toInsert.Any())
                {
                    userProfileRepository.InsertMany(toInsert);
                }
            }
        }

        public string GetProfileEntry(int userId, string key)
        {
            var entry = GetProfilenInformations(userId).FirstOrDefault(x => x.Key == key);

            if (entry != null)
            {
                return entry.Value;
            }

            return null;
        }

        public void SaveProfileEntry(int userId, string key, string value)
        {
            var entry = GetProfilenInformations(userId).FirstOrDefault(x => x.Key == key);

            if (entry != null)
            {
                entry.Value = value;
                userProfileRepository.Update(entry);
            }
            else
            {
                userProfileRepository.Insert(new UserProfileEntry
                {
                    Id = 0, 
                    UserId = userId,
                    Key = key,
                    Value = value
                });
            }
        }

        public void DeleteProfileEntry(int userId, string key)
        {
            var entry = GetProfilenInformations(userId).FirstOrDefault(x => x.Key == key);

            if (entry != null)
            {
                userProfileRepository.Delete(entry);
            }
        }

        public IEnumerable<UserProfileEntry> GetProfileEntriesByKey(string key)
        {
            return userProfileRepository.Table.Where(x => x.Key == key).ToHashSet();
        }

        public List<User> SearchUserPaged(
            string searchText, 
            int pageIndex,
            int pageSize, 
            out int totalRecord)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@SearchText", searchText),
                AddInputParameter("@PageIndex", pageIndex),
                AddInputParameter("@PageSize", pageSize)
            };

            return ExecuteReader<User>("sp_User_Search_Paged", "@TotalRecord", out totalRecord, list.ToArray());
        }

        public List<User> SearchUserByRolePaged(
            string searchText,
            int roleId,
            int pageIndex,
            int pageSize,
            out int totalRecord)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@SearchText", searchText),
                AddInputParameter("@RoleId", roleId),  
                AddInputParameter("@PageIndex", pageIndex),
                AddInputParameter("@PageSize", pageSize)
            };

            return ExecuteReader<User>("sp_User_ByRoles_Paged", "@TotalRecord", out totalRecord, list.ToArray());
        }

        #endregion Profile

        protected override IOrderedQueryable<User> MakeDefaultOrderBy(IQueryable<User> queryable)
        {
            return queryable.OrderBy(x => x.UserName);
        }

		#region IMembershipService Members


		public IEnumerable<User> GetUsersByRole(int roleId)
		{
			var list = usersInRoleRepository.Table.Where(x => x.Id == roleId);
			if (list.Any())
			{
				var listIds = list.Select(y => y.UserId).ToList();
				return userRepository.Table.Where(x => listIds.Contains(x.Id)).ToList();
			}

			return null;
		}

		#endregion
	}
}