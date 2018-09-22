using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Media.Connectors.Responses;
using CMSSolutions.ContentManagement.Media.Models;
using CMSSolutions.ContentManagement.Media.Services;
using CMSSolutions.FileSystems.Media;
using CMSSolutions.Web.Mvc;

namespace CMSSolutions.ContentManagement.Media.Connectors
{
    public class MediaSystemDriver : IDriver
    {
        private readonly IStorageProvider storageProvider;
        private readonly IMediaService mediaService;
        private readonly IMimeTypeProvider mimeTypeProvider;
        private static readonly DateTime unixOrigin = new DateTime(1970, 1, 1, 0, 0, 0);

        public MediaSystemDriver(IMediaService mediaService, IMimeTypeProvider mimeTypeProvider, IStorageProvider storageProvider)
        {
            this.mediaService = mediaService;
            this.mimeTypeProvider = mimeTypeProvider;
            this.storageProvider = storageProvider;
        }

        private static JsonResult Json(object data)
        {
            return new JsonDataContractResult(data) { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult Open(string target, bool tree)
        {
            if (string.IsNullOrEmpty(target))
            {
                return Init(target);
            }

            var path = Helper.DecodePath(target).Trim(Path.DirectorySeparatorChar);

            if (string.IsNullOrEmpty(path))
            {
                return Init(target);
            }

            var folder = mediaService.GetMediaFolder(path);
            var folders = mediaService.GetMediaFolders(path);
            var files = mediaService.GetMediaFiles(path);

            var response = new OpenResponse(CreateDto(folder, Path.GetDirectoryName(path)));

            foreach (var file in files)
            {
                response.AddResponse(CreateDto(file));
            }

            foreach (var child in folders)
            {
                response.AddResponse(CreateDto(child, path));
            }
            return Json(response);
        }

        public JsonResult Init(string target)
        {
            var folders = mediaService.GetMediaFolders(null);
            var files = mediaService.GetMediaFiles(null);

            var directory = new RootDto
            {
                Mime = "directory",
                Dirs = folders.Any() ? (byte)1 : (byte)0,
                Hash = Helper.EncodePath("\\"),
                Locked = 0,
                Name = "Media",
                Read = 1,
                Write = 1,
                Size = 0,
                UnixTimeStamp = (long)(DateTime.UtcNow - unixOrigin).TotalSeconds,
                VolumeId = "v1"
            };

            var response = new InitResponse(directory);

            foreach (var file in files)
            {
                response.AddResponse(CreateDto(file));
            }

            foreach (var folder in folders)
            {
                response.AddResponse(CreateDto(folder, null));
            }

            response.Options.Path = "Media";
            response.Options.Url = "/";
            response.Options.ThumbnailsUrl = "/";

            return Json(response);
        }

        public JsonResult Parents(string target)
        {
            //var id = new Guid(Helper.DecodePath(target));
            //var folders = mediaService.GetMediaFolders(id);
            //var answer = new TreeResponse();
            //return Json(answer);
            throw new NotSupportedException();
        }

        public JsonResult Tree(string target)
        {
            var path = Helper.DecodePath(target).Trim(Path.DirectorySeparatorChar);
            var parentPath = string.IsNullOrEmpty(path) ? null : Path.GetDirectoryName(path);
            var folders = mediaService.GetMediaFolders(path);
            var answer = new TreeResponse();
            foreach (var folder in folders)
            {
                answer.Tree.Add(CreateDto(folder, parentPath));
            }
            return Json(answer);
        }

        public JsonResult List(string target)
        {
            var path = Helper.DecodePath(target);
            var files = mediaService.GetMediaFiles(path);
            var answer = new ListResponse();

            foreach (var file in files)
            {
                answer.List.Add(file.Name);
            }

            return Json(answer);
        }

        public JsonResult MakeDir(string target, string name)
        {
            var path = Helper.DecodePath(target).Trim(Path.DirectorySeparatorChar);
            var folder = mediaService.CreateFolder(path, name);
            return Json(new AddResponse(CreateDto(folder, path)));
        }

        public JsonResult MakeFile(string target, string name)
        {
            throw new NotSupportedException();
        }

        public JsonResult Rename(string target, string name)
        {
            if (!string.IsNullOrEmpty(target))
            {
                var path = Helper.DecodePath(target).Replace("\\", "/").Replace(@"\", "/").Replace("//", "/");
                if (string.IsNullOrEmpty(Path.GetExtension(path)))
                {
                    throw new NotImplementedException("Cannot found process rename folder.");
                }
                var index = path.LastIndexOf('/');
                var folder = string.Empty;
                var fileName = string.Empty;
                if (index > -1)
                {
                    folder = path.Substring(0, index);
                    fileName = path.Replace(folder, string.Empty);
                }

                var answer = new ReplaceResponse();
                mediaService.RenameFile(folder, fileName, name);

                answer.Removed.Add(target);
                var relativePath = storageProvider.Combine(folder, name);
                var file = storageProvider.GetFile(relativePath);
                var mediaFile = new MediaFile
                {
                    Name = name,
                    Size = file.GetSize(),
                    LastUpdated = file.GetLastUpdated(),
                    Type = file.GetFileType(),
                    FolderName = folder,
                    MediaPath = mediaService.GetMediaPublicUrl(folder, name)
                };
                answer.Added.Add(CreateDto(mediaFile));

                return Json(answer);
            }

            throw new NotImplementedException();
        }

        public JsonResult Remove(IEnumerable<string> targets)
        {
            var answer = new RemoveResponse();
            foreach (var item in targets)
            {
                var path = Helper.DecodePath(item);

                if (mediaService.FileExists(path))
                {
                    mediaService.DeleteFile(path);
                }
                else if (mediaService.FolderExists(path))
                {
                    mediaService.DeleteFolder(path);
                }

                answer.Removed.Add(item);
            }
            return Json(answer);
        }

        public JsonResult Duplicate(IEnumerable<string> targets)
        {
            throw new NotSupportedException();
        }

        public JsonResult Get(string target)
        {
            throw new NotImplementedException();
            //var id = new Guid(Helper.DecodePath(target));
            //var file = mediaService.GetMediaFile(id);
            //if (file == null)
            //{
            //    return null;
            //}
            //return Json(new MediaFileResult(mediaService, file));
        }

        public JsonResult Put(string target, string content)
        {
            throw new NotSupportedException();
        }

        public JsonResult Paste(string source, string dest, IEnumerable<string> targets, bool isCut)
        {
            throw new NotImplementedException();
            //var destinationId = new Guid(Helper.DecodePath(dest));
            //var response = new ReplaceResponse();

            //foreach (var item in targets)
            //{
            //    var target = new Guid(Helper.DecodePath(item));

            //    var folder = mediaService.GetMediaFolder(target);
            //    if (folder != null)
            //    {
            //        folder.ParentId = destinationId;
            //        mediaService.UpdateMediaFolder(folder);
            //        response.Removed.Add(item);
            //    }
            //    else
            //    {
            //        var file = mediaService.GetMediaFile(target);
            //        if (file != null)
            //        {
            //            file.ParentId = destinationId;
            //            mediaService.UpdateMediaFile(file);
            //            response.Removed.Add(item);
            //        }
            //    }
            //}
            //return Json(response);
        }

        public JsonResult Upload(string target, HttpFileCollectionBase targets)
        {
            var path = Helper.DecodePath(target).Trim(Path.DirectorySeparatorChar);

            var response = new AddResponse();

            for (int i = 0; i < targets.AllKeys.Length; i++)
            {
                var file = targets[i];
                if (file != null && file.ContentLength > 0)
                {
                    mediaService.UploadMediaFile(path, file.FileName, file.InputStream);
                    var mediaFile = mediaService.GetMediaFile(path, file.FileName);
                    response.Added.Add(CreateDto(mediaFile));
                }
            }
            return Json(response);
        }

        private static DtoBase CreateDto(MediaFolder folder, string folderName)
        {
            var response = new DirectoryDto
            {
                Mime = "directory",
                ContainsChildDirs = 1,
                Hash = Helper.EncodePath(folder.MediaPath),
                Locked = 0,
                Read = 1,
                Write = 1,
                Size = 0,
                Name = folder.Name,
                UnixTimeStamp = (long)(DateTime.UtcNow - unixOrigin).TotalSeconds,
                ParentHash = string.IsNullOrEmpty(folderName) ? Helper.EncodePath("\\") : Helper.EncodePath(folderName)
            };
            return response;
        }

        private DtoBase CreateDto(MediaFile file)
        {
            var hash = Helper.EncodePath(string.IsNullOrEmpty(file.FolderName) ? file.Name : storageProvider.Combine(file.FolderName, file.Name));
            var response = new FileDto
            {
                Read = 1,
                Write = 1,
                Locked = 0,
                Name = file.Name,
                Size = file.Size,
                UnixTimeStamp = (long)(file.LastUpdated - unixOrigin).TotalSeconds,
                Mime = mimeTypeProvider.GetMimeType(file.Name),
                Hash = hash,
                ParentHash = string.IsNullOrEmpty(file.FolderName) ? Helper.EncodePath("\\") : Helper.EncodePath(file.FolderName),
                Url = file.MediaPath
            };
            return response;
        }
    }
}