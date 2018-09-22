using System.IO;
using System.Text;
using System.Web.Hosting;
using CMSSolutions.FileSystems.VirtualPath;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlFormVirtualPathProvider : VirtualPathProvider, ICustomVirtualPathProvider
    {
        public override bool FileExists(string virtualPath)
        {
            if (virtualPath.Contains("ControlFormResult_"))
            {
                return true;
            }
            return base.FileExists(virtualPath);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (virtualPath.Contains("ControlFormResult_"))
            {
                return new ControlFormVirtualFile(virtualPath);
            }

            return base.GetFile(virtualPath);
        }

        public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, System.DateTime utcStart)
        {
            if (virtualPath.Contains("ControlFormResult_"))
            {
                return null;
            }
            return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        #region Nested type: ControlFormVirtualFile

        private class ControlFormVirtualFile : VirtualFile
        {
            public ControlFormVirtualFile(string virtualPath)
                : base(virtualPath)
            {
            }

            public override Stream Open()
            {
                return new MemoryStream(Encoding.UTF8.GetBytes("@inherits CMSSolutions.Web.Mvc.WebViewPage<dynamic>\r\n[THIS_IS_CONTENT_HOLDER_FOR_ROBO_FORM]"));
            }
        }

        #endregion Nested type: ControlFormVirtualFile

        public VirtualPathProvider Instance { get { return this; } }
    }
}