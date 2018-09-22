using System.Collections.Generic;
using CMSSolutions.Environment.Extensions.Models;

namespace CMSSolutions.Environment.Extensions.Folders
{
    public interface IExtensionFolders
    {
        IEnumerable<ExtensionDescriptor> AvailableExtensions();
    }
}