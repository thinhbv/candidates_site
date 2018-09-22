using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using CMSSolutions.Configuration;
using CMSSolutions.Extensions;
using CMSSolutions.FileSystems.AppData;
using CMSSolutions.Localization;
using CMSSolutions.Security.Cryptography;

namespace CMSSolutions.Environment.Configuration
{
    public class ShellSettingsManager : IShellSettingsManager
    {
        private readonly IAppDataFolder appDataFolder;
        private readonly IShellSettingsManagerEventHandler events;

        public Localizer T { get; set; }

        public ShellSettingsManager(
            IAppDataFolder appDataFolder,
            IShellSettingsManagerEventHandler events)
        {
            this.appDataFolder = appDataFolder;
            this.events = events;

            T = NullLocalizer.Instance;
        }

        IEnumerable<ShellSettings> IShellSettingsManager.LoadSettings()
        {
            var settings = LoadSettings().ToList();

            if (settings.Count == 0)
            {
                // Default shell settings
                if (string.IsNullOrEmpty(CMSConfigurationSection.Instance.Data.SettingConnectionString))
                {
                    throw new ArgumentException("Does not have default connection string in web.config.");
                }

                var connectionString = ConfigurationManager.ConnectionStrings[CMSConfigurationSection.Instance.Data.SettingConnectionString].ConnectionString;
                if (KeyConfiguration.IsEncrypt)
                {
                    connectionString = EncryptionExtensions.Decrypt(CMSConfigurationSection.Instance.Data.SettingConnectionString, connectionString);
                }

                var shellSettings = new ShellSettings
                {
                    Name = ShellSettings.DefaultName,
                    DataProvider = "SqlServer",
                    DataConnectionString = connectionString,
                    State = TenantState.Running,
                    EncryptionAlgorithm = "AES",
                    HashAlgorithm = "HMACSHA256"
                };
                shellSettings.EncryptionKey = SymmetricAlgorithm.Create(shellSettings.EncryptionAlgorithm).Key.ToHexString();
                shellSettings.HashKey = HMAC.Create(shellSettings.HashAlgorithm).Key.ToHexString();

                // Save tenant settings
                ((IShellSettingsManager)this).SaveSettings(shellSettings);

                settings.Add(shellSettings);
            }
            else
            {
                var defaultSettings = settings.FirstOrDefault(x => x.Name == ShellSettings.DefaultName);
                if (defaultSettings != null)
                {
                    // Default shell settings
                    if (!string.IsNullOrEmpty(CMSConfigurationSection.Instance.Data.SettingConnectionString))
                    {
                        var connectionString = ConfigurationManager.ConnectionStrings[CMSConfigurationSection.Instance.Data.SettingConnectionString].ConnectionString;
                        if (KeyConfiguration.IsEncrypt)
                        {
                            connectionString = EncryptionExtensions.Decrypt(CMSConfigurationSection.Instance.Data.SettingConnectionString, connectionString);
                        }
                        if (!string.IsNullOrEmpty(connectionString))
                        {
                            defaultSettings.DataConnectionString = connectionString;        
                        }
                    }
                }
            }

            return settings;
        }

        void IShellSettingsManager.SaveSettings(ShellSettings settings)
        {
            if (settings == null)
                throw new ArgumentException(T("There are no settings to save.").ToString());

            if (string.IsNullOrEmpty(settings.Name))
                throw new ArgumentException(T("Settings \"Name\" is not set.").ToString());

            var filePath = Path.Combine(Path.Combine("Sites", settings.Name), "Settings.txt");
            appDataFolder.CreateFile(filePath, ShellSettingsSerializer.ComposeSettings(settings));
            events.Saved(settings);
        }

        private IEnumerable<ShellSettings> LoadSettings()
        {
            var filePaths = appDataFolder
                .ListDirectories("Sites")
                .SelectMany(path => appDataFolder.ListFiles(path))
                .Where(path => string.Equals(Path.GetFileName(path), "Settings.txt", StringComparison.OrdinalIgnoreCase));

            foreach (var filePath in filePaths)
            {
                yield return ShellSettingsSerializer.ParseSettings(appDataFolder.ReadFile(filePath));
            }
        }
    }
}