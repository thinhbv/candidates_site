﻿using System.Collections.Generic;
using System.IO;
using System.Web;
using CMSSolutions.ContentManagement.Media.Models;
using CMSSolutions.Data;

namespace CMSSolutions.ContentManagement.Media.Services
{
    public interface IMediaService : IDependency
    {
        /// <summary>
        /// Creates a unique filename to prevent filename collisions.
        /// </summary>
        /// <param name="folderPath">The relative where collisions will be checked.</param>
        /// <param name="filename">The desired filename.</param>
        /// <returns>A string representing a unique filename.</returns>
        string GetUniqueFilename(string folderPath, string filename);

        /// <summary>
        /// Returns the public URL for a media file.
        /// </summary>
        /// <param name="mediaPath">The relative path of the media folder containing the media.</param>
        /// <param name="fileName">The media file name.</param>
        /// <returns>The public URL for the media.</returns>
        string GetMediaPublicUrl(string mediaPath, string fileName);

        IEnumerable<string> GetMediaFolders();

        /// <summary>
        /// Retrieves the media folders within a given relative path.
        /// </summary>
        /// <param name="relativePath">The path where to retrieve the media folder from. null means root.</param>
        /// <returns>The media folder in the given path.</returns>
        IEnumerable<MediaFolder> GetMediaFolders(string relativePath);

        /// <summary>
        /// Retrieves the media files within a given relative path.
        /// </summary>
        /// <param name="relativePath">The path where to retrieve the media files from. null means root.</param>
        /// <returns>The media files in the given path.</returns>
        IEnumerable<MediaFile> GetMediaFiles(string relativePath);

        IList<TMediaPart> GetMediaParts<TKey, TMediaPart>(BaseEntity<TKey> entity) where TMediaPart : IMediaPart, new();

        bool FolderExists(string path);

        MediaFolder GetMediaFolder(string folderPath);

        /// <summary>
        /// Creates a media folder.
        /// </summary>
        /// <param name="relativePath">The path where to create the new folder. null means root.</param>
        /// <param name="folderName">The name of the folder to be created.</param>
        MediaFolder CreateFolder(string relativePath, string folderName);

        /// <summary>
        /// Deletes a media folder.
        /// </summary>
        /// <param name="folderPath">The path to the folder to be deleted.</param>
        void DeleteFolder(string folderPath);

        /// <summary>
        /// Renames a media folder.
        /// </summary>
        /// <param name="folderPath">The path to the folder to be renamed.</param>
        /// <param name="newFolderName">The new folder name.</param>
        void RenameFolder(string folderPath, string newFolderName);

        bool FileExists(string path);

        MediaFile GetMediaFile(string folderPath, string fileName);

        /// <summary>
        /// Deletes a media file.
        /// </summary>
        /// <param name="path">The file path.</param>
        void DeleteFile(string path);

        /// <summary>
        /// Deletes a media file.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="fileName">The file name.</param>
        void DeleteFile(string folderPath, string fileName);

        /// <summary>
        /// Renames a media file.
        /// </summary>
        /// <param name="folderPath">The path to the file's parent folder.</param>
        /// <param name="currentFileName">The current file name.</param>
        /// <param name="newFileName">The new file name.</param>
        void RenameFile(string folderPath, string currentFileName, string newFileName);

        /// <summary>
        /// Moves a media file.
        /// </summary>
        /// <param name="currentPath">The path to the file's parent folder.</param>
        /// <param name="filename">The file name.</param>
        /// <param name="newPath">The path where the file will be moved to.</param>
        /// <param name="newFilename">The new file name.</param>
        void MoveFile(string currentPath, string filename, string newPath, string newFilename);

        void MoveFile(string currentPath, string newPath);

        void MoveFiles(IEnumerable<IMediaPart> mediaParts, string targetFolder);

        /// <summary>
        /// Uploads a media file based on a posted file.
        /// </summary>
        /// <param name="folderPath">The path to the folder where to upload the file.</param>
        /// <param name="postedFile">The file to upload.</param>
        /// <returns>The path to the uploaded file.</returns>
        string UploadMediaFile(string folderPath, HttpPostedFileBase postedFile);

        /// <summary>
        /// Uploads a media file based on an array of bytes.
        /// </summary>
        /// <param name="folderPath">The path to the folder where to upload the file.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="bytes">The array of bytes with the file's contents.</param>
        /// <returns>The path to the uploaded file.</returns>
        string UploadMediaFile(string folderPath, string fileName, byte[] bytes);

        /// <summary>
        /// Uploads a media file based on a stream.
        /// </summary>
        /// <param name="folderPath">The folder path to where to upload the file.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="inputStream">The stream with the file's contents.</param>
        /// <returns>The path to the uploaded file.</returns>
        string UploadMediaFile(string folderPath, string fileName, Stream inputStream);

        void SetMediaParts<TKey>(BaseEntity<TKey> entity, IEnumerable<IMediaPart> mediaParts, string folderName);
    }
}