using System;
using Castle.Core.Logging;
using CMSSolutions.Caching;
using CMSSolutions.Environment.Configuration;
using CMSSolutions.FileSystems.AppData;

namespace CMSSolutions.Environment
{
    public interface IHostLocalRestart
    {
        /// <summary>
        /// Monitor changes on the persistent storage.
        /// </summary>
        void Monitor(Action<IVolatileToken> monitor);
    }

    public class DefaultHostLocalRestart : IHostLocalRestart, IShellSettingsManagerEventHandler
    {
        private readonly IAppDataFolder appDataFolder;
        private const string FileName = "hrestart.txt";

        public DefaultHostLocalRestart(IAppDataFolder appDataFolder)
        {
            this.appDataFolder = appDataFolder;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public int Priority { get { return int.MaxValue; } }

        public void Monitor(Action<IVolatileToken> monitor)
        {
            if (!appDataFolder.FileExists(FileName))
                TouchFile();

            monitor(appDataFolder.WhenPathChanges(FileName));
        }

        void IShellSettingsManagerEventHandler.Saved(ShellSettings settings)
        {
            TouchFile();
        }

        private void TouchFile()
        {
            try
            {
                appDataFolder.CreateFile(FileName, "Host Restart");
            }
            catch (Exception e)
            {
                Logger.WarnFormat(e, "Error updating file '{0}'", FileName);
            }
        }
    }
}