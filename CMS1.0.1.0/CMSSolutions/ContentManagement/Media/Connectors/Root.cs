using System;
using System.IO;

namespace CMSSolutions.ContentManagement.Media.Connectors
{
    /// <summary>
    /// Represents a root of file system
    /// </summary>
    public class Root
    {
        /// <summary>
        /// Initialize new instanse of class CMSSolutions.ContentManagement.Media.Services.Root
        /// </summary>
        /// <param name="directory">Directory which will be root</param>
        /// <param name="url">Url to root</param>
        public Root(DirectoryInfo directory, string url)
        {
            if (directory == null)
                throw new ArgumentNullException("directory", "Root directory can not be null");
            if (!directory.Exists)
                throw new ArgumentException("Root directory must exist", "directory");
            Alias = directory.Name;
            Directory = directory;
            TmbPath = ".thumbnails";
            TmbSize = 48;
            TmbBgColor = "#ff0000";
            Url = TmbUrl = url ?? string.Empty;
        }

        /// <summary>
        /// Initialize new instanse of class CMSSolutions.ContentManagement.Media.Services.Root
        /// </summary>
        /// <param name="directory">Directory which will be root</param>
        public Root(DirectoryInfo directory)
            : this(directory, string.Empty)
        {
        }

        public string VolumeId { get; internal set; }

        /// <summary>
        /// Get or sets root alias
        /// </summary>
        public string Alias { get; set; }

        public DirectoryInfo Directory { get; private set; }

        public string Url { get; private set; }

        public string TmbUrl { get; set; }

        public string TmbPath { get; set; }

        public int TmbSize { get; set; }

        public string TmbBgColor { get; set; }

        public bool IsReadOnly { get; set; }
    }
}