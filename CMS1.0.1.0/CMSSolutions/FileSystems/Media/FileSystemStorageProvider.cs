using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using CMSSolutions.Environment;
using CMSSolutions.Localization;

namespace CMSSolutions.FileSystems.Media
{
    public class FileSystemStorageProvider : IStorageProvider
    {
        private readonly string storagePath;
        private readonly string virtualPath;
        private readonly string publicPath;

        public FileSystemStorageProvider(ShellSettings settings)
        {
            var configMediaFolder = System.Configuration.ConfigurationManager.AppSettings[settings.Name + "MediaFolder"];
            if (!string.IsNullOrEmpty(configMediaFolder))
            {
                virtualPath = configMediaFolder;
                storagePath = HostingEnvironment.MapPath(virtualPath);
                publicPath = configMediaFolder.Trim('~');
            }
            else
            {
                var mediaPath = HostingEnvironment.IsHosted
                                ? HostingEnvironment.MapPath("~/Media/") ?? ""
                                : Combine(AppDomain.CurrentDomain.BaseDirectory, "Media");

                storagePath = Combine(mediaPath, settings.Name);
                virtualPath = "~/Media/" + settings.Name + "/";

                var appPath = "";
                if (HostingEnvironment.IsHosted)
                {
                    appPath = HostingEnvironment.ApplicationVirtualPath ?? "";
                }

                if (!appPath.EndsWith("/"))
                    appPath = appPath + '/';
                if (!appPath.StartsWith("/"))
                    appPath = '/' + appPath;

                publicPath = appPath + "Media/" + settings.Name + "/";
            }

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        /// <summary>
        /// Maps a relative path into the storage path.
        /// </summary>
        /// <param name="path">The relative path to be mapped.</param>
        /// <returns>The relative path combined with the storage path.</returns>
        private string MapStorage(string path)
        {
            string mappedPath = string.IsNullOrEmpty(path) ? storagePath : Combine(storagePath, path);
            return PathValidation.ValidatePath(storagePath, mappedPath);
        }

        /// <summary>
        /// Maps a relative path into the public path.
        /// </summary>
        /// <param name="path">The relative path to be mapped.</param>
        /// <returns>The relative path combined with the public path in an URL friendly format ('/' character for directory separator).</returns>
        private string MapPublic(string path)
        {
            return string.IsNullOrEmpty(path) ? publicPath : Combine(publicPath, path).Replace(Path.DirectorySeparatorChar, '/');
        }

        private static string Fix(string path)
        {
            return string.IsNullOrEmpty(path)
                       ? ""
                       : Path.DirectorySeparatorChar != '/'
                             ? path.Replace('/', Path.DirectorySeparatorChar)
                             : path;
        }

        #region Implementation of IStorageProvider

        /// <summary>
        /// Checks if the given file exists within the storage provider.
        /// </summary>
        /// <param name="path">The relative path within the storage provider.</param>
        /// <returns>True if the file exists; False otherwise.</returns>
        public bool FileExists(string path)
        {
            return File.Exists(MapStorage(path));
        }

        /// <summary>
        /// Retrieves the public URL for a given file within the storage provider.
        /// </summary>
        /// <param name="path">The relative path within the storage provider.</param>
        /// <returns>The public URL.</returns>
        public string GetPublicUrl(string path)
        {
            return MapPublic(path);
        }

        public string GetStoragePath()
        {
            return storagePath;
        }

        /// <summary>
        /// Retrieves the path within the storage provider for a given public url.
        /// </summary>
        /// <param name="url">The virtual or public url of a media.</param>
        /// <returns>The storage path or <value>null</value> if the media is not in a correct format.</returns>
        public string GetStoragePath(string url)
        {
            if (url.StartsWith(virtualPath))
            {
                return url.Substring(virtualPath.Length).Replace('/', Path.DirectorySeparatorChar).Replace("%20", " ");
            }

            if (url.StartsWith(publicPath))
            {
                return url.Substring(publicPath.Length).Replace('/', Path.DirectorySeparatorChar).Replace("%20", " ");
            }

            return null;
        }

        /// <summary>
        /// Retrieves a file within the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file within the storage provider.</param>
        /// <returns>The file.</returns>
        /// <exception cref="ArgumentException">If the file is not found.</exception>
        public IStorageFile GetFile(string path)
        {
            FileInfo fileInfo = new FileInfo(MapStorage(path));
            if (!fileInfo.Exists)
            {
                throw new ArgumentException(T("File {0} does not exist", path).ToString());
            }

            return new FileSystemStorageFile(Fix(path), fileInfo);
        }

        /// <summary>
        /// Lists the files within a storage provider's path.
        /// </summary>
        /// <param name="path">The relative path to the folder which files to list.</param>
        /// <returns>The list of files in the folder.</returns>
        public IEnumerable<IStorageFile> ListFiles(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(MapStorage(path));
            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException(T("Directory {0} does not exist", MapStorage(path)).ToString());
            }

            return directoryInfo
                .GetFiles()
                .Where(fi => !IsHidden(fi))
                .Select<FileInfo, IStorageFile>(fi => new FileSystemStorageFile(Combine(Fix(path), fi.Name), fi))
                .ToList();
        }

        /// <summary>
        /// Checks if the given folder exists within the storage provider.
        /// </summary>
        /// <param name="path">The relative path within the storage provider.</param>
        /// <returns>True if the folder exists; False otherwise.</returns>
        public bool FolderExists(string path)
        {
            return new DirectoryInfo(MapStorage(path)).Exists;
        }

        public IStorageFolder GetFolder(string path)
        {
            var fullPath = MapStorage(path);

            if (Directory.Exists(fullPath))
            {
                return new FileSystemStorageFolder(path, new DirectoryInfo(fullPath));
            }

            return null;
        }

        public IEnumerable<string> ListFolders()
        {
            var directoryInfo = new DirectoryInfo(MapStorage(null));
            if (!directoryInfo.Exists)
            {
                try
                {
                    directoryInfo.Create();
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(T("The folder could not be created at path: {0}. {1}", "/", ex).ToString());
                }
            }

            return directoryInfo.GetDirectories("*", SearchOption.AllDirectories).Select(x => x.FullName.Substring(storagePath.Length + 1)).OrderBy(x => x).ToArray();
        }

        /// <summary>
        /// Lists the folders within a storage provider's path.
        /// </summary>
        /// <param name="path">The relative path to the folder which folders to list.</param>
        /// <returns>The list of folders in the folder.</returns>
        public IEnumerable<IStorageFolder> ListFolders(string path)
        {
            var directoryInfo = new DirectoryInfo(MapStorage(path));
            if (!directoryInfo.Exists)
            {
                try
                {
                    directoryInfo.Create();
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(T("The folder could not be created at path: {0}. {1}", path, ex).ToString());
                }
            }

            return directoryInfo
                .GetDirectories()
                .Where(di => !IsHidden(di))
                .Select<DirectoryInfo, IStorageFolder>(di => new FileSystemStorageFolder(Combine(Fix(path), di.Name), di))
                .ToList();
        }

        /// <summary>
        /// Tries to create a folder in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the folder to be created.</param>
        /// <returns>True if success; False otherwise.</returns>
        public bool TryCreateFolder(string path)
        {
            try
            {
                // prevent unnecessary exception
                DirectoryInfo directoryInfo = new DirectoryInfo(MapStorage(path));
                if (directoryInfo.Exists)
                {
                    return false;
                }

                CreateFolder(path);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a folder in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the folder to be created.</param>
        /// <exception cref="ArgumentException">If the folder already exists.</exception>
        public IStorageFolder CreateFolder(string path)
        {
            var directoryInfo = new DirectoryInfo(MapStorage(path));
            if (directoryInfo.Exists)
            {
                throw new ArgumentException(T("Directory {0} already exists", path).ToString());
            }

            directoryInfo = Directory.CreateDirectory(directoryInfo.FullName);
            return new FileSystemStorageFolder(path, directoryInfo);
        }

        /// <summary>
        /// Deletes a folder in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the folder to be deleted.</param>
        /// <exception cref="ArgumentException">If the folder doesn't exist.</exception>
        public void DeleteFolder(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(MapStorage(path));
            if (!directoryInfo.Exists)
            {
                throw new ArgumentException(T("Directory {0} does not exist", path).ToString());
            }

            directoryInfo.Delete(true);
        }

        /// <summary>
        /// Renames a folder in the storage provider.
        /// </summary>
        /// <param name="oldPath">The relative path to the folder to be renamed.</param>
        /// <param name="newPath">The relative path to the new folder.</param>
        public void RenameFolder(string oldPath, string newPath)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(MapStorage(oldPath));
            if (!sourceDirectory.Exists)
            {
                throw new ArgumentException(T("Directory {0} does not exist", oldPath).ToString());
            }

            DirectoryInfo targetDirectory = new DirectoryInfo(MapStorage(newPath));
            if (targetDirectory.Exists)
            {
                throw new ArgumentException(T("Directory {0} already exists", newPath).ToString());
            }

            Directory.Move(sourceDirectory.FullName, targetDirectory.FullName);
        }

        /// <summary>
        /// Deletes a file in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file to be deleted.</param>
        /// <exception cref="ArgumentException">If the file doesn't exist.</exception>
        public void DeleteFile(string path)
        {
            var fileInfo = new FileInfo(MapStorage(path));
            if (!fileInfo.Exists)
            {
                throw new ArgumentException(T("File {0} does not exist", path).ToString());
            }

            fileInfo.Delete();
        }

        /// <summary>
        /// Renames a file in the storage provider.
        /// </summary>
        /// <param name="oldPath">The relative path to the file to be renamed.</param>
        /// <param name="newPath">The relative path to the new file.</param>
        public void RenameFile(string oldPath, string newPath)
        {
            FileInfo sourceFileInfo = new FileInfo(MapStorage(oldPath));
            if (!sourceFileInfo.Exists)
            {
                throw new ArgumentException(T("File {0} does not exist", oldPath).ToString());
            }

            FileInfo targetFileInfo = new FileInfo(MapStorage(newPath));
            if (targetFileInfo.Exists)
            {
                throw new ArgumentException(T("File {0} already exists", newPath).ToString());
            }

            File.Move(sourceFileInfo.FullName, targetFileInfo.FullName);
        }

        /// <summary>
        /// Creates a file in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file to be created.</param>
        /// <exception cref="ArgumentException">If the file already exists.</exception>
        /// <returns>The created file.</returns>
        public IStorageFile CreateFile(string path)
        {
            FileInfo fileInfo = new FileInfo(MapStorage(path));
            if (fileInfo.Exists)
            {
                throw new ArgumentException(T("File {0} already exists", fileInfo.Name).ToString());
            }

            // ensure the directory exists
            var dirName = Path.GetDirectoryName(fileInfo.FullName);
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            File.WriteAllBytes(fileInfo.FullName, new byte[0]);

            return new FileSystemStorageFile(Fix(path), fileInfo);
        }

        /// <summary>
        /// Tries to save a stream in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file to be created.</param>
        /// <param name="inputStream">The stream to be saved.</param>
        /// <returns>True if success; False otherwise.</returns>
        public bool TrySaveStream(string path, Stream inputStream)
        {
            try
            {
                SaveStream(path, inputStream);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Saves a stream in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file to be created.</param>
        /// <param name="inputStream">The stream to be saved.</param>
        /// <exception cref="ArgumentException">If the stream can't be saved due to access permissions.</exception>
        public void SaveStream(string path, Stream inputStream)
        {
            // Create the file.
            // The CreateFile method will map the still relative path
            var file = CreateFile(path);

            var outputStream = file.OpenWrite();
            var buffer = new byte[8192];
            for (; ; )
            {
                var length = inputStream.Read(buffer, 0, buffer.Length);
                if (length <= 0)
                    break;
                outputStream.Write(buffer, 0, length);
            }
            outputStream.Dispose();
        }

        /// <summary>
        /// Combines to paths.
        /// </summary>
        /// <param name="path1">The parent path.</param>
        /// <param name="path2">The child path.</param>
        /// <returns>The combined path.</returns>
        public string Combine(string path1, string path2)
        {
            if (!string.IsNullOrEmpty(path1))
            {
                if ((path1.EndsWith(@"\", System.StringComparison.Ordinal)) || path1.EndsWith(@"/", System.StringComparison.Ordinal))
                {
                    return (path1 + path2).Replace("//", "/");
                }
            }

            if (!string.IsNullOrEmpty(path2))
            {
                if ((path2.EndsWith(@"\", System.StringComparison.Ordinal)) || path2.EndsWith(@"/", System.StringComparison.Ordinal))
                {
                    return path1 + path2;
                }
            }
            
            return path1 + "/" + path2;
        }

        private static bool IsHidden(FileSystemInfo di)
        {
            return (di.Attributes & FileAttributes.Hidden) != 0;
        }

        #endregion Implementation of IStorageProvider

        private class FileSystemStorageFile : IStorageFile
        {
            private readonly string path;
            private readonly FileInfo fileInfo;

            public FileSystemStorageFile(string path, FileInfo fileInfo)
            {
                this.path = path;
                this.fileInfo = fileInfo;
            }

            #region Implementation of IStorageFile

            public string GetPath()
            {
                return path;
            }

            public string GetName()
            {
                return fileInfo.Name;
            }

            public long GetSize()
            {
                return fileInfo.Length;
            }

            public DateTime GetLastUpdated()
            {
                return fileInfo.LastWriteTime;
            }

            public string GetFileType()
            {
                return fileInfo.Extension;
            }

            public Stream OpenRead()
            {
                return new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            }

            public Stream OpenWrite()
            {
                return new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite);
            }

            public Stream CreateFile()
            {
                return new FileStream(fileInfo.FullName, FileMode.Truncate, FileAccess.ReadWrite);
            }

            #endregion Implementation of IStorageFile
        }

        private class FileSystemStorageFolder : IStorageFolder
        {
            private readonly string path;
            private readonly DirectoryInfo directoryInfo;

            public FileSystemStorageFolder(string path, DirectoryInfo directoryInfo)
            {
                this.path = path;
                this.directoryInfo = directoryInfo;

                T = NullLocalizer.Instance;
            }

            public Localizer T { get; set; }

            #region Implementation of IStorageFolder

            public string GetPath()
            {
                return path;
            }

            public string GetName()
            {
                return directoryInfo.Name;
            }

            public DateTime GetLastUpdated()
            {
                return directoryInfo.LastWriteTime;
            }

            public long GetSize()
            {
                return GetDirectorySize(directoryInfo);
            }

            public IStorageFolder GetParent()
            {
                if (directoryInfo.Parent != null)
                {
                    return new FileSystemStorageFolder(Path.GetDirectoryName(path), directoryInfo.Parent);
                }
                throw new ArgumentException(T("Directory {0} does not have a parent directory", directoryInfo.Name).ToString());
            }

            #endregion Implementation of IStorageFolder

            private static long GetDirectorySize(DirectoryInfo directoryInfo)
            {
                long size = 0;

                FileInfo[] fileInfos = directoryInfo.GetFiles();
                foreach (FileInfo fileInfo in fileInfos)
                {
                    if (!IsHidden(fileInfo))
                    {
                        size += fileInfo.Length;
                    }
                }
                DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
                foreach (DirectoryInfo dInfo in directoryInfos)
                {
                    if (!IsHidden(dInfo))
                    {
                        size += GetDirectorySize(dInfo);
                    }
                }

                return size;
            }
        }
    }
}