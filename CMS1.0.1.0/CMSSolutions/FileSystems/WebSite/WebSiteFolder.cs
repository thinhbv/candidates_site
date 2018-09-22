using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using CMSSolutions.Caching;
using CMSSolutions.FileSystems.VirtualPath;

namespace CMSSolutions.FileSystems.WebSite
{
    public class WebSiteFolder : IWebSiteFolder
    {
        private readonly IVirtualPathProvider virtualPathProvider;
        private readonly IVirtualPathMonitor virtualPathMonitor;

        public WebSiteFolder(IVirtualPathMonitor virtualPathMonitor, IVirtualPathProvider virtualPathProvider)
        {
            this.virtualPathMonitor = virtualPathMonitor;
            this.virtualPathProvider = virtualPathProvider;
        }

        public IEnumerable<string> ListDirectories(string virtualPath)
        {
            if (!virtualPathProvider.DirectoryExists(virtualPath))
            {
                return Enumerable.Empty<string>();
            }

            return virtualPathProvider.ListDirectories(virtualPath);
        }

        private IEnumerable<string> ListFiles(IEnumerable<string> directories)
        {
            return directories.SelectMany(d => ListFiles(d, true));
        }

        public IEnumerable<string> ListFiles(string virtualPath, bool recursive)
        {
            if (!recursive)
            {
                return virtualPathProvider.ListFiles(virtualPath);
            }
            return virtualPathProvider.ListFiles(virtualPath).Concat(ListFiles(ListDirectories(virtualPath)));
        }

        public bool FileExists(string virtualPath)
        {
            return virtualPathProvider.FileExists(virtualPath);
        }

        public string ReadFile(string virtualPath)
        {
            return ReadFile(virtualPath, false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public string ReadFile(string virtualPath, bool actualContent)
        {
            if (!virtualPathProvider.FileExists(virtualPath))
            {
                return null;
            }

            if (actualContent)
            {
                var physicalPath = virtualPathProvider.MapPath(virtualPath);
                using (var stream = File.Open(physicalPath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }

            using (var stream = virtualPathProvider.OpenFile(Normalize(virtualPath)))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public void CopyFileTo(string virtualPath, Stream destination)
        {
            CopyFileTo(virtualPath, destination, false/*actualContent*/);
        }

        public void CopyFileTo(string virtualPath, Stream destination, bool actualContent)
        {
            if (actualContent)
            {
                // This is an unfortunate side-effect of the dynamic compilation work.
                // System has a custom virtual path provider which adds "<@Assembly xxx@>"
                // directives to WebForm view files. There are cases when this side effect
                // is not expected by the consumer of the WebSiteFolder API.
                // The workaround here is to go directly to the file system.
                var physicalPath = virtualPathProvider.MapPath(virtualPath);
                using (var stream = File.Open(physicalPath, FileMode.Open, FileAccess.Read))
                {
                    stream.CopyTo(destination);
                }
            }
            else
            {
                using (var stream = virtualPathProvider.OpenFile(Normalize(virtualPath)))
                {
                    stream.CopyTo(destination);
                }
            }
        }

        public IVolatileToken WhenPathChanges(string virtualPath)
        {
            return virtualPathMonitor.WhenPathChanges(virtualPath);
        }

        private static string Normalize(string virtualPath)
        {
            // todo: use IVirtualPathProvider instance instead of static.
            // Currently IVirtualPathProvider has no way of doing normalization like this
            return HostingEnvironment.VirtualPathProvider.GetFile(virtualPath).VirtualPath;
        }
    }
}