using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Configuration;
using CMSSolutions.Configuration;
using CMSSolutions.Data;
using CMSSolutions.Environment;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Security.Cryptography;
using CMSSolutions.Web.Security.Domain;

namespace CMSSolutions.Web.Security
{
    [Feature(Constants.Areas.Security)]
    public class StartupTask : IStartupTask
    {
        private readonly IRepository<User, int> userRepository;
        private readonly IRepository<LocalAccount, int> localAccountRepository;

        public StartupTask(IRepository<User, int> userRepository, IRepository<LocalAccount, int> localAccountRepository)
        {
            this.userRepository = userRepository;
            this.localAccountRepository = localAccountRepository;
        }

        public void Run()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings[CMSConfigurationSection.Instance.Data.SettingConnectionString].ConnectionString;
                var connecttion = new SqlConnection
                {
                    ConnectionString = connectionString
                };
                try
                {
                    var filePath = System.AppDomain.CurrentDomain.BaseDirectory + @"SqlScript\sp_default.sql";
                    string script = File.ReadAllText(filePath);
                    string[] splitter = new string[] { "\r\nGO\r\n" };
                    IEnumerable<string> commandStrings = script.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                    connecttion.Open();
                    foreach (string commandString in commandStrings)
                    {
                        if (commandString.Trim() != "")
                        {
                            using (var command = new SqlCommand(commandString, connecttion))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    connecttion.Close();
                }
                catch (Exception ex)
                {
                    if (connecttion.State == ConnectionState.Open)
                    {
                        connecttion.Close();
                    }

                    throw new Exception(ex.Message);
                }

                var user = userRepository.Table.FirstOrDefault(x => x.UserName == Constants.DefaultUserName);
                if (user == null)
                {
                    using (var transaction = new TransactionScope())
                    {
                        user = new User
                        {
                            Id = 0,
                            UserName = Constants.DefaultUserName,
                            Email = Constants.DefaultUserName + "@yourdomain.com",
                            SuperUser = true,
                            CreateDate = DateTime.UtcNow
                        };
                        userRepository.Insert(user);

                        string hashedPassword = EncryptionExtensions.Encrypt(KeyConfiguration.PublishKey, Constants.DefaultPassword);
                        if (string.IsNullOrEmpty(hashedPassword))
                        {
                            throw new ArgumentException(SecurityConstants.ErrorConfigKey);
                        }

                        var localAccount = new LocalAccount
                        {
                            UserId = user.Id,
                            IsConfirmed = true,
                            Password = hashedPassword
                        };
                        localAccountRepository.Insert(localAccount);
                        transaction.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
               
            }
        }
    }
}
