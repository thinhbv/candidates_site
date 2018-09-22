using System.Web.Hosting;

namespace CMSSolutions.FileSystems.VirtualPath
{
    public interface ICustomVirtualPathProvider
    {
        VirtualPathProvider Instance { get; }
    }
}