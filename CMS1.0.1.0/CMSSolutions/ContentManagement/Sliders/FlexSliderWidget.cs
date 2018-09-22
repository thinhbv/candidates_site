using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using JetBrains.Annotations;
using CMSSolutions.ContentManagement.Media.Models;
using CMSSolutions.ContentManagement.Media.Services;
using CMSSolutions.ContentManagement.Widgets;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Extensions;
using CMSSolutions.Web.UI;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Sliders
{
    [Feature(Constants.Areas.Media)]
    public class FlexSliderWidget : WidgetBase
    {
        public override string Name
        {
            get { return "Flex Slider Widget"; }
        }

        [ControlText(MaxLength = 10, ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public string Width { get; set; }

        [ControlText(MaxLength = 10, ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public string Height { get; set; }

        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Slider")]
        public Guid SliderId { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "Control Navigation", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public bool ControlNavigation { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "Show Thumbnails", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public bool ShowThumbnails { get; set; }

        [ControlGrid(1, 10, 5, CssClass = "table table-striped table-bordered", ContainerCssClass = "col-xs-12 col-sm-12", ContainerRowIndex = 4)]
        public List<MediaPart> UploadPhotos { get; set; }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            if (UploadPhotos == null || UploadPhotos.Count == 0)
            {
                return;
            }

            var clientId = "flexSlider_" + Guid.NewGuid().ToString("N").ToLowerInvariant();

            if (ShowTitleOnPage)
            {
                writer.RenderBeginTag("header");
                writer.RenderBeginTag(HtmlTextWriterTag.H3);
                writer.Write(Title);
                writer.RenderEndTag(); // h3    
                writer.RenderEndTag(); // header   
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Id, clientId);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "flexslider");
            writer.AddStyleAttributeIfHave("max-width", Width);
            writer.AddStyleAttributeIfHave("max-height", Height);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "slides");
            writer.RenderBeginTag(HtmlTextWriterTag.Ul);

            foreach (var slide in UploadPhotos)
            {
                if (ShowThumbnails)
                {
                    writer.AddAttribute("data-thumb", slide.Url);
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Li);

                writer.AddAttribute(HtmlTextWriterAttribute.Href, string.IsNullOrEmpty(slide.TargetUrl) ? "javascript:void(0)" : slide.TargetUrl);
                writer.RenderBeginTag(HtmlTextWriterTag.A);

                writer.AddAttribute(HtmlTextWriterAttribute.Src, slide.Url);
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, slide.Caption);
                writer.AddAttribute(HtmlTextWriterAttribute.Title, slide.Caption);
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag(); // img

                writer.RenderEndTag(); // a

                writer.RenderEndTag(); //li
            }

            writer.RenderEndTag(); // ul

            writer.RenderEndTag(); // div
            
            writer.Write("<script type=\"text/javascript\">");
            writer.Write("$(document).ready(function() {{ $('#{0}').flexslider({{ animation: \"slide\", controlNav: {1}, directionNav: true }}); }});", clientId, ShowThumbnails ? "\"thumbnails\"" : ControlNavigation ? "true" : "false");
            writer.Write("</script>");
        }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            yield return ResourceType.FlexSlider;
        }

        public override void OnSaving(WorkContext workContext)
        {
            if (UploadPhotos != null)
            {
                UploadPhotos.RemoveAll(x => string.IsNullOrEmpty(x.Url));

                if (UploadPhotos.Count > 0)
                {
                    var mediaService = workContext.Resolve<IMediaService>();
                    mediaService.MoveFiles(new List<IMediaPart>(UploadPhotos), "Slideshow");
                }
            }
        }

        [UsedImplicitly]
        public class MediaPart : IMediaPart
        {
            [ControlFileUpload(EnableFineUploader = true, Required = true, ColumnWidth = 300)]
            public string Url { get; set; }

            [ControlText(MaxLength = 255)]
            public string Caption { get; set; }

            [ControlText(MaxLength = 2048)]
            public string TargetUrl { get; set; }

            [ControlNumeric(Required = true, LabelText = "Sort Order")]
            public int SortOrder { get; set; }
        }
    }
}
