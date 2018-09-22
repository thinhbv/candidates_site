using System;
using System.Linq;
using Autofac;
using Castle.Core.Logging;
using CMSSolutions.Environment.Configuration;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.ShellBuilders;
using CMSSolutions.Tasks;

namespace CMSSolutions.ContentManagement.Backups
{
    [Feature(Constants.Areas.Backups)]
    public class BackupDatabasesTask : IScheduleTask
    {
        public string Name { get { return "Backup Databases Task"; } }

        public bool Enabled { get { return false; } }

        public string CronExpression { get { return "0 0 23 1/1 * ? *"; } }

        public bool DisallowConcurrentExecution { get { return true; } }

        public void Execute(IWorkContextScope scope)
        {
            var logger = scope.WorkContext.ResolveWithParameters<ILogger>(new TypedParameter(typeof(Type), typeof(BackupDatabasesTask)));
            logger.Info("Starting execute Backup Databases Task.");

            var backupStorageProvider = scope.Resolve<IBackupStorageProvider>();
            if (backupStorageProvider == null)
            {
                logger.Error("Does not have any backup storage provider in system.");
                return;
            }

            var shellSettingsManager = scope.Resolve<IShellSettingsManager>();
            var shellContextFactory = scope.Resolve<IShellContextFactory>();
            var allSettings = shellSettingsManager.LoadSettings().ToArray();
            var folder = "Backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

            foreach (var shellSettings in allSettings)
            {
                try
                {
                    var shellContext = shellContextFactory.CreateShellContext(shellSettings);
                    var workContextScope = shellContext.LifetimeScope.CreateWorkContextScope();
                    var backupProvider = workContextScope.Resolve<IBackupProvider>();

                    if (backupProvider == null)
                    {
                        logger.Error("Does not have any backup provider in system.");
                        continue;
                    }

                    string fileName;
                    var stream = backupProvider.Backup(out fileName);
                    if (stream == null)
                    {
                        continue;
                    }

                    backupStorageProvider.Store(stream, folder, fileName, scope);
                }
                catch (Exception ex)
                {
                    logger.InfoFormat(ex, "Cannot backup database for {0} tenant.", shellSettings.Name);
                }
            }
        }
    }
}