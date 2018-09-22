using System;
using System.IO;
using System.Web.Mvc;

namespace CMSSolutions.ContentManagement.Media.Models
{
    [ModelBinder(typeof(ModelBinder))]
    public class FineUpload
    {
        public Guid Id { get; set; }

        public string Filename { get; set; }

        public Stream InputStream { get; set; }

        public string ContentType { get; set; }

        public class ModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var request = controllerContext.RequestContext.HttpContext.Request;
                var formUpload = request.Files.Count > 0;

                var uploadFile = formUpload ? request.Files[0] : null;

                // find filename
                var xFileName = request.Headers["X-File-Name"];
                var qqFile = request["qqfile"];
                var qquuid = request["qquuid"];

                // ReSharper disable PossibleNullReferenceException
                var formFilename = formUpload ? uploadFile.FileName : null;
                // ReSharper restore PossibleNullReferenceException

                var contentType = formUpload ? uploadFile.ContentType : request["Content-Type"];

                var upload = new FineUpload
                {
                    Id = !string.IsNullOrEmpty(qquuid) ? new Guid(qquuid) : Guid.NewGuid(),

                    Filename = xFileName ?? qqFile ?? formFilename,

                    // ReSharper disable PossibleNullReferenceException
                    InputStream = formUpload ? uploadFile.InputStream : request.InputStream,

                    // ReSharper restore PossibleNullReferenceException
                    ContentType = contentType
                };

                return upload;
            }
        }
    }
}