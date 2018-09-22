using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Lists.Fields;
using CMSSolutions.ContentManagement.Media.Models;
using CMSSolutions.ContentManagement.Media.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Media
{
    [Feature(Constants.Areas.Media)]
    public class PhotosListField : BaseListField
    {
        public PhotosListField()
        {
            MaxPhotos = 10;
        }

        public override string FieldType
        {
            get { return "Photos Field"; }
        }

        [ControlNumeric(Required = true, MinimumValue = "0", LabelText = "Min Photos")]
        public byte MinPhotos { get; set; }

        [ControlNumeric(Required = true, MinimumValue = "1", MaximumValue = "255", LabelText = "Max Photos")]
        public byte MaxPhotos { get; set; }

        [ControlText(Required = true, MaxLength = 255, LabelText = "Photos Folder")]
        public string PhotosFolder { get; set; }

        public override void BindControlField(ControlFormResult controlForm, object value)
        {
            var attribute = new ControlGridAttribute(MinPhotos, MaxPhotos)
                                {
                                    PropertyName = Name,
                                    LabelText = Title,
                                    PropertyType = typeof(List<MediaPart>),
                                    ShowLabelControl = false,
                                    EnabledScroll = true
                                };

            controlForm.AddProperty(Name, attribute, value);
        }

        public override object GetControlFormValue(Controller controller, WorkContext workContext)
        {
            var model = new List<MediaPart>();

            var binder = new DefaultModelBinder();
            var bindingContext = new ModelBindingContext
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelState = controller.ViewData.ModelState,
                ValueProvider = controller.ValueProvider,
                ModelName = Name
            };

            binder.BindModel(controller.ControllerContext, bindingContext);

            // Move media files to correct folder
            if (model.Count > 0)
            {
                var mediaService = workContext.Resolve<IMediaService>();
                mediaService.MoveFiles(model, PhotosFolder);
            }

            model.Sort((x, y) => x.SortOrder.CompareTo(y.SortOrder));

            return model;
        }

        public override object RenderField(object value, object[] args)
        {
            var model = value as List<MediaPart>;
            if (model == null || model.Count == 0)
            {
                return null;
            }

            string arg = null;
            var relGroup = Guid.NewGuid().ToString("N");

            if (args != null && args.Length > 0)
            {
                arg = Convert.ToString(args[0]);
            }

            if (arg == "List")
            {
                return MvcHtmlString.Create(string.Format("<img src=\"{0}\" title=\"{1}\" />", model[0].Url, model[0].Caption));
            }

            if (arg == "List-Url")
            {
                return MvcHtmlString.Create(model[0].Url);
            }

            var isFancybox = arg == "Fancybox";

            var sb = new StringBuilder();
            var clientId = "ul_" + Guid.NewGuid().ToString("N").ToLower();

            sb.AppendFormat("<ul data-orbit data-options=\"resume_on_mouseout: true; timer_speed: 5000;\" id=\"{0}\">", clientId);

            foreach (var mediaPart in model)
            {
                sb.Append("<li>");
                if (isFancybox)
                {
                    sb.AppendFormat("<a class=\"fancybox\" rel=\"g_{2}\" href=\"{0}\"><img src=\"{0}\" alt=\"{1}\" /></a><div class=\"orbit-caption\">{1}</div>", mediaPart.Url, mediaPart.Caption, relGroup);
                }
                else
                {
                    sb.AppendFormat("<img src=\"{0}\" alt=\"{1}\" /><div class=\"orbit-caption\">{1}</div>", mediaPart.Url, mediaPart.Caption);
                }

                sb.Append("</li>");
            }

            sb.Append("</ul>");

            return MvcHtmlString.Create(sb.ToString());
        }

        public class MediaPart : IMediaPart
        {
            [ControlFileUpload(EnableFineUploader = true, ColumnWidth = 250, Required = true, ShowThumbnail = true, Order = 0)]
            public string Url { get; set; }

            [ControlText(MaxLength = 255, Order = 1)]
            public string Caption { get; set; }

            [ControlNumeric(Required = true, LabelText = "Sort Order", Order = 2)]
            public int SortOrder { get; set; }
        }
    }
}