using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using CMSSolutions.FileSystems.VirtualPath;

namespace CMSSolutions.Environment
{
    public interface IBuildManager : IDependency
    {
        IEnumerable<Assembly> GetReferencedAssemblies();

        bool HasReferencedAssembly(string name);

        Assembly GetReferencedAssembly(string name);

        Assembly GetCompiledAssembly(string virtualPath);
    }

    public class DefaultBuildManager : IBuildManager
    {
        private readonly IVirtualPathProvider virtualPathProvider;
        private readonly IAssemblyLoader assemblyLoader;

        public DefaultBuildManager(
            IVirtualPathProvider virtualPathProvider,
            IAssemblyLoader assemblyLoader)
        {
            this.virtualPathProvider = virtualPathProvider;
            this.assemblyLoader = assemblyLoader;
        }

        public IEnumerable<Assembly> GetReferencedAssemblies()
        {
            return BuildManager.GetReferencedAssemblies().OfType<Assembly>();
        }

        public bool HasReferencedAssembly(string name)
        {
            var assemblyPath = virtualPathProvider.Combine("~/bin", name + ".dll");
            return virtualPathProvider.FileExists(assemblyPath);
        }

        public Assembly GetReferencedAssembly(string name)
        {
            if (!HasReferencedAssembly(name))
                return null;

            return assemblyLoader.Load(name);
        }

        public Assembly GetCompiledAssembly(string virtualPath)
        {
            try
            {
                return BuildManager.GetCompiledAssembly(virtualPath);
            }
            catch
            {
                return null;
            }
        }
    }
}