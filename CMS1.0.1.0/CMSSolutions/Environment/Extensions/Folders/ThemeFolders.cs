using System.Collections.Generic;
using CMSSolutions.Environment.Extensions.Models;

namespace CMSSolutions.Environment.Extensions.Folders
{
    public class ThemeFolders : IExtensionFolders
    {
        private readonly IEnumerable<string> paths;
        private readonly IExtensionHarvester extensionHarvester;

        public ThemeFolders(IEnumerable<string> paths, IExtensionHarvester extensionHarvester)
        {
            this.paths = paths;
            this.extensionHarvester = extensionHarvester;
        }

        public IEnumerable<ExtensionDescriptor> AvailableExtensions()
        {
            return extensionHarvester.HarvestExtensions(paths, DefaultExtensionTypes.Theme, "Theme.txt", false/*isManifestOptional*/);
        }
    }
}