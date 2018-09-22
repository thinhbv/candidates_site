using System;
using System.Reflection;
using Castle.Core.Logging;
using CMSSolutions.Environment;
using CMSSolutions.FileSystems.AppData;

namespace CMSSolutions.FileSystems.Dependencies
{
    public class DefaultAssemblyProbingFolder : IAssemblyProbingFolder
    {
        private const string BasePath = "Dependencies";
        private readonly IAppDataFolder appDataFolder;
        private readonly IAssemblyLoader assemblyLoader;

        public DefaultAssemblyProbingFolder(IAppDataFolder appDataFolder, IAssemblyLoader assemblyLoader)
        {
            this.appDataFolder = appDataFolder;
            this.assemblyLoader = assemblyLoader;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public bool AssemblyExists(string moduleName)
        {
            var path = PrecompiledAssemblyPath(moduleName);
            return appDataFolder.FileExists(path);
        }

        public DateTime GetAssemblyDateTimeUtc(string moduleName)
        {
            var path = PrecompiledAssemblyPath(moduleName);
            return appDataFolder.GetFileLastWriteTimeUtc(path);
        }

        public string GetAssemblyVirtualPath(string moduleName)
        {
            var path = PrecompiledAssemblyPath(moduleName);
            if (!appDataFolder.FileExists(path))
                return null;

            return appDataFolder.GetVirtualPath(path);
        }

        public Assembly LoadAssembly(string moduleName)
        {
            var path = PrecompiledAssemblyPath(moduleName);
            if (!appDataFolder.FileExists(path))
                return null;

            return assemblyLoader.Load(moduleName);
        }

        public void DeleteAssembly(string moduleName)
        {
            var path = PrecompiledAssemblyPath(moduleName);

            if (appDataFolder.FileExists(path))
            {
                Logger.InfoFormat("Deleting assembly for module \"{0}\" from probing directory", moduleName);
                appDataFolder.DeleteFile(path);
            }
        }

        public void StoreAssembly(string moduleName, string fileName)
        {
            var path = PrecompiledAssemblyPath(moduleName);

            Logger.InfoFormat("Storing assembly file \"{0}\" for module \"{1}\"", fileName, moduleName);
            appDataFolder.StoreFile(fileName, path);
        }

        private string PrecompiledAssemblyPath(string moduleName)
        {
            return appDataFolder.Combine(BasePath, moduleName + ".dll");
        }
    }
}