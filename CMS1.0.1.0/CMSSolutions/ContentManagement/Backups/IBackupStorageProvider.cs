using System.IO;

namespace CMSSolutions.ContentManagement.Backups
{
    public interface IBackupStorageProvider : IDependency
    {
        void Store(Stream stream, string folder, string fileName, IWorkContextScope scope);

        Stream Get(string name);
    }
}