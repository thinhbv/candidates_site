using System.Collections.Generic;
using CMSSolutions.Environment.Extensions.Models;

namespace CMSSolutions.Environment.Extensions.Folders
{
    public interface IExtensionHarvester
    {
        IEnumerable<ExtensionDescriptor> HarvestExtensions(IEnumerable<string> paths, string extensionType, string manifestName, bool manifestIsOptional);
    }
}