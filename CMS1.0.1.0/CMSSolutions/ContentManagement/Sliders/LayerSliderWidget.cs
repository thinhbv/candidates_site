using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using System.Web.UI;
using Newtonsoft.Json.Linq;
using CMSSolutions.Collections;
using CMSSolutions.ContentManagement.Sliders.Services;
using CMSSolutions.ContentManagement.Widgets;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Sliders
{
    [Feature(Constants.Areas.Sliders)]
    public class LayerSliderWidget : WidgetBase
    {
        public override string Name
        {
            get { return "Layer Slider Widget"; }
        }

        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Slider", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public Guid SliderId { get; set; }

        [ControlChoice(ControlChoice.DropDownList, Required = true, ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public string Skin { get; set; }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            if (SliderId == Guid.Empty)
            {
                return;
            }

            var workContext = workContextAccessor.GetContext(viewContext.HttpContext);
            var sliderService = workContext.Resolve<ISliderService>();
            var slider = sliderService.GetById(SliderId);
            if (slider == null)
            {
                return;
            }

            var slideService = workContext.Resolve<ISlideService>();
            var slides = slideService.GetSlides(SliderId);
            if (slides.Count == 0)
            {
                return;
            }
            var urlHelper = workContext.Resolve<UrlHelper>();
            writer.Write("<div id=\"layerslider-{0}\" style=\"width: {1}px; height: {2}px;\">", Id, slider.Width, slider.Height);

            foreach (var slide in slides)
            {
                if (slide.SlideDelay.HasValue)
                {
                    writer.AddStyleAttribute("slidedelay", slide.SlideDelay.Value.ToString(CultureInfo.InvariantCulture));
                }
                if (!string.IsNullOrEmpty(slide.Transition2D))
                {
                    writer.AddStyleAttribute("transition2d", slide.Transition2D);
                }
                if (!string.IsNullOrEmpty(slide.Transition3D))
                {
                    writer.AddStyleAttribute("transition3d", slide.Transition3D);
                }
                writer.AddStyleAttribute("slidedirection", slide.SlideDirection);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "ls-layer");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (!string.IsNullOrEmpty(slide.BackgroundUrl))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "ls-bg");
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, urlHelper.Content(slide.BackgroundUrl));

                    if (!string.IsNullOrEmpty(slide.Title))
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Alt, slide.Title);
                    }

                    if (!string.IsNullOrEmpty(slide.OnClick))
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Onclick, slide.OnClick);
                    }

                    writer.RenderBeginTag(HtmlTextWriterTag.Img);
                    writer.RenderEndTag();
                }

                writer.Write(slide.Sublayers);

                writer.RenderEndTag();
            }

            writer.Write("</div>");

            var options = new JObject
            {
            // ReSharper disable Html.PathError
                {"skinsPath", urlHelper.Content("~/Styles/LayerSlider/Skins/")},
            // ReSharper restore Html.PathError
                {"responsive", true},
                {"navPrevNext", false},
                {"skin", Skin}
            };

            writer.WriteLine("<script type=\"text/javascript\">");
            writer.WriteLine("$(document).ready(function(){");
            writer.Write("$('#layerslider-{0}').layerSlider({1});", Id, options);
            writer.WriteLine("});");
            writer.Write("</script>");
        }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            yield return ResourceType.LayerSlider;
        }

        public override ActionResult BuildEditor(Controller controller, WorkContext workContext, ControlFormResult<IWidget> controlForm)
        {
            var sliderService = workContext.Resolve<ISliderService>();
            var sliders = sliderService.GetRecords();
            controlForm.RegisterExternalDataSource("SliderId", sliders.ToSelectList(v => v.Id.ToString(), t => t.Name));

            controlForm.RegisterExternalDataSource("Skin", "defaultskin", "fullwidth", "glass", "borderlessdark", "borderlesslight", "darkskin", "lightskin", "minimal");

            return base.BuildEditor(controller, workContext, controlForm);
        }
    }
}