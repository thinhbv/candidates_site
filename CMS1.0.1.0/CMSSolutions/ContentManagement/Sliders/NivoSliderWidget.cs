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
    public class NivoSliderWidget : WidgetBase
    {
        public override string Name
        {
            get { return "Nivo Slider Widget"; }
        }

        [ControlChoice(ControlChoice.DropDownList, Required = true, ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public string Theme { get; set; }

        [ControlChoice(ControlChoice.DropDownList, Required = true, ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public string Effect { get; set; }

        [ControlText(ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public string Width { get; set; }

        [ControlText(ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public string Height { get; set; }

        [ControlNumeric(Required = true, MinimumValue = "1000", LabelText = "Pause Time", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 4)]
        public int PauseTime { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "Control Navigation", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 4)]
        public bool ControlNavigation { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "Show Thumbnails", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 4)]
        public bool ShowThumbnails { get; set; }

        [ControlGrid(1, 10, 5, CssClass = "table table-striped table-bordered", ContainerCssClass = "col-xs-12 col-sm-12", ContainerRowIndex = 5)]
        public List<MediaPart> UploadPhotos { get; set; }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            if (UploadPhotos == null || UploadPhotos.Count == 0)
            {
                return;
            }

            var clientId = "nivoSlider_" + Guid.NewGuid().ToString("N").ToLowerInvariant();

            if (ShowTitleOnPage)
            {
                writer.RenderBeginTag("header");
                writer.RenderBeginTag(HtmlTextWriterTag.H3);
                writer.Write(Title);
                writer.RenderEndTag(); // h3    
                writer.RenderEndTag(); // header    
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "slider-wrapper " + Theme);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Id, clientId);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "nivoSlider");
            writer.AddStyleAttributeIfHave("max-width", Width);
            writer.AddStyleAttributeIfHave("max-height", Height);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            foreach (var slide in UploadPhotos)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Href, string.IsNullOrEmpty(slide.TargetUrl) ? "javascript:void(0)" : slide.TargetUrl);
                writer.RenderBeginTag(HtmlTextWriterTag.A);

                writer.AddAttribute(HtmlTextWriterAttribute.Src, slide.Url);
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, slide.Caption);
                writer.AddAttribute(HtmlTextWriterAttribute.Title, slide.Caption);
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag(); // img

                writer.RenderEndTag(); // a
            }

            writer.RenderEndTag(); // div

            writer.RenderEndTag(); // div

            var workContext = workContextAccessor.GetContext();
            var scripRegister = new ScriptRegister(workContext);
            scripRegister.IncludeInline(string.Format("$(document).ready(function(){{ $('#{0}').nivoSlider({{ controlNavThumbs: {2}, controlNav: {1}, effect: '{3}', pauseTime: {4} }}); }});",
                clientId,
                ControlNavigation ? "true" : "false",
                ShowThumbnails ? "true" : "false",
                Effect, PauseTime));
        }

        public override ActionResult BuildEditor(Controller controller, WorkContext workContext, ControlFormResult<IWidget> form)
        {
            form.RegisterExternalDataSource("Theme", new Dictionary<string, string>
                                                         {
                                                             {"theme-default", "Default"},
                                                             {"theme-bar", "Bar"},
                                                             {"theme-dark", "Dark"},
                                                             {"theme-light", "Light"},
                                                         });

            form.RegisterExternalDataSource("Effect", new Dictionary<string, string>
                                                         {
                                                             {"random", "Random"},
                                                             {"sliceDown", "Slice Down"},
                                                             {"sliceDownLeft", "Slice Down Left"},
                                                             {"sliceUp", "Slice Up"},
                                                             {"sliceUpLeft", "Slice Up Left"},
                                                             {"sliceUpDown", "Slice Up Down"},
                                                             {"sliceUpDownLeft", "Slice Up Down Left"},
                                                             {"fold", "Fold"},
                                                             {"fade", "Fade"},
                                                             {"slideInRight", "Slide In Right"},
                                                             {"slideInLeft", "Slide In Left"},
                                                             {"boxRandom", "Box Random"},
                                                             {"boxRain", "Box Rain"},
                                                             {"boxRainReverse", "Box Rain Reverse"},
                                                             {"boxRainGrow", "Box Rain Grow"},
                                                             {"boxRainGrowReverse", "Box Rain Grow Reverse"},
                                                         });

            return base.BuildEditor(controller, workContext, form);
        }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            yield return ResourceType.NivoSlider;
        }

        public override void OnSaving(WorkContext workContext)
        {
            if (UploadPhotos != null)
            {
                UploadPhotos.RemoveAll(x => string.IsNullOrEmpty(x.Url));

                if (UploadPhotos.Count > 0)
                {
                    var mediaService = workContext.Resolve<IMediaService>();
                    mediaService.MoveFiles(UploadPhotos, "Slideshow\\" + Title);
                }
            }
        }

        [UsedImplicitly]
        public class MediaPart : IMediaPart
        {
            [ControlFileUpload(EnableFineUploader = true, Required = true, ColumnWidth = 500, ShowThumbnail = true)]
            public string Url { get; set; }

            [ControlText(MaxLength = 255)]
            public string Caption { get; set; }

            [ControlText(MaxLength = 2048, LabelText = "Target Url")]
            public string TargetUrl { get; set; }

            [ControlNumeric(Required = true, LabelText = "Sort Order", ColumnWidth = 100)]
            public int SortOrder { get; set; }
        }
    }
}
