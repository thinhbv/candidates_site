using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using CMSSolutions.ContentManagement.Media.Connectors;
using CMSSolutions.ContentManagement.Media.Models;
using CMSSolutions.ContentManagement.Media.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;

namespace CMSSolutions.ContentManagement.Media.Controllers
{
    [Authorize]
    [Feature(Constants.Areas.Media)]
    public class UploadFilesController : Controller
    {
        private readonly IMediaService mediaService;

        public UploadFilesController(IMediaService mediaService)
        {
            this.mediaService = mediaService;
        }

        [Url("media-library/upload-files", Priority = 0)]
        public ActionResult UploadFiles(FineUpload fineUpload)
        {
            var fileName = fineUpload.Filename;
            var folder = Request.Form["folder"];
            if (string.IsNullOrEmpty(folder))
            {
                folder = "UploadFiles";
            }

            if (mediaService.FileExists(folder + "\\" + fileName))
            {
                fileName = mediaService.GetUniqueFilename(folder, fileName);
            }

            var mediaUrl = mediaService.UploadMediaFile(folder, fileName, fineUpload.InputStream);
            var newUuid = Helper.EncodePath(Path.Combine(folder, fileName));
            var result = new { mediaUrl, newUuid };

            return new FineUploaderResult(true, result);
        }

        [AcceptVerbs(HttpVerbs.Delete | HttpVerbs.Post)]
        [Url("media-library/delete-upload-file/{id}")]
        public ActionResult DeleteUploadFile(string id)
        {
            var path = Helper.DecodePath(id);
            mediaService.DeleteFile(path);
            return new FineUploaderResult(true);
        }

        private class FineUploaderResult : ActionResult
        {
            private const string ResponseContentType = "text/plain";

            private readonly bool success;
            private readonly string error;
            private readonly bool? preventRetry;
            private readonly JObject otherData;

            public FineUploaderResult(bool success, object otherData = null, string error = null, bool? preventRetry = null)
            {
                this.success = success;
                this.error = error;
                this.preventRetry = preventRetry;

                if (otherData != null)
                    this.otherData = JObject.FromObject(otherData);
            }

            public override void ExecuteResult(ControllerContext context)
            {
                var response = context.HttpContext.Response;
                response.ContentType = ResponseContentType;
                response.Write(BuildResponse());
            }

            private string BuildResponse()
            {
                var response = otherData ?? new JObject();
                response["success"] = success;

                if (!string.IsNullOrWhiteSpace(error))
                    response["error"] = error;

                if (preventRetry.HasValue)
                    response["preventRetry"] = preventRetry.Value;

                return response.ToString();
            }
        }
    }
}