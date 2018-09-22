using System.IO;

namespace CMSSolutions.ContentManagement.Backups
{
    public interface IBackupProvider : IDependency
    {
        Stream Backup(out string fileName);
    }
}