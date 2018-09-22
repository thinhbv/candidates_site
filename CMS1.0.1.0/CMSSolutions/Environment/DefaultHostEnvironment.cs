using System;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;

namespace CMSSolutions.Environment
{
    public class DefaultHostEnvironment : IHostEnvironment
    {
        #region IHostEnvironment Members

        public bool IsFullTrust
        {
            get { return AppDomain.CurrentDomain.IsHomogenous && AppDomain.CurrentDomain.IsFullyTrusted; }
        }

        public string MapPath(string virtualPath)
        {
            return HostingEnvironment.MapPath(virtualPath);
        }

        public bool IsAssemblyLoaded(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Any(assembly => new AssemblyName(assembly.FullName).Name == name);
        }

        public void RestartAppDomain()
        {
        }

        #endregion IHostEnvironment Members
    }
}