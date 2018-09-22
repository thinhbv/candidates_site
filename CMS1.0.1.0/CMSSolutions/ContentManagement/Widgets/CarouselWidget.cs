using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Widgets
{
    [Feature(Constants.Areas.Widgets)]
    public class CarouselWidget : WidgetBase
    {
        public CarouselWidget()
        {
            VisibleItems = 5;
            PlaySpeed = 5000;
            EnablePagination = true;
        }

        public override string Name
        {
            get { return "Carousel Widget"; }
        }

        [ControlNumeric(Required = true, MinimumValue = "1", LabelText = "Visible Items", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public int VisibleItems { get; set; }

        [ControlNumeric(Required = true, MinimumValue = "0", LabelText = "Play Speed", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public int PlaySpeed { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "&nbsp;", AppendText = "Stop On Hover", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public bool StopOnHover { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "&nbsp;", AppendText = "Enable Pagination", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public bool EnablePagination { get; set; }

        [ControlGrid(0, 20, 5, CssClass = "table table-bordered", ContainerCssClass = "col-xs-12 col-sm-12", ContainerRowIndex = 4)]
        public IList<CarouselImage> Images { get; set; }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            yield return ResourceType.OwlCarousel;
        }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            if (Images == null || Images.Count == 0)
            {
                return;
            }

            if (!string.IsNullOrEmpty(CssClass))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (ShowTitleOnPage)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.H3);
                writer.Write(Title);
                writer.RenderEndTag(); // h3
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Id, "owl-" + Id);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            foreach (var image in Images)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "item");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (!string.IsNullOrEmpty(image.Url))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, image.Url);
                    writer.AddAttribute(HtmlTextWriterAttribute.Target, "_blank");
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                }

                writer.AddAttribute(HtmlTextWriterAttribute.Src, image.ImageUrl);
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, image.Title);
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag(); // img

                if (!string.IsNullOrEmpty(image.Url))
                {
                    writer.RenderEndTag(); // a
                }

                if (!string.IsNullOrEmpty(image.Title))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "caption");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "title");
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write(image.Title);
                    writer.RenderEndTag();

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "sub-title");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    if (!string.IsNullOrEmpty(image.Url))
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, image.Url);
                        writer.AddAttribute(HtmlTextWriterAttribute.Target, "_blank");
                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                    }

                    writer.Write(image.SubTitle);

                    if (!string.IsNullOrEmpty(image.Url))
                    {
                        writer.RenderEndTag(); // a
                    }

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "description");
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write(image.Description);
                    writer.RenderEndTag(); // span

                    writer.RenderEndTag(); // div

                    writer.RenderEndTag(); // div
                }
                
                writer.RenderEndTag(); // div
            }

            writer.RenderEndTag(); // div

            writer.RenderEndTag(); // div

            var workContext = workContextAccessor.GetContext(viewContext.HttpContext);
            var scriptRegister = new ScriptRegister(workContext);

            var options = new JObject
                          {
                              {"items", VisibleItems},
                              {"stopOnHover", StopOnHover},
                              {"pagination", EnablePagination}
                          };

            if (PlaySpeed > 0)
            {
                options.Add("autoPlay", PlaySpeed);
            }
            else
            {
                options.Add("autoPlay", false);
            }

            scriptRegister.IncludeInline(string.Format("$(document).ready(function(){{ $('#owl-{0}').owlCarousel({1}); }});", Id, options.ToString(Formatting.None)));
        }

        public class CarouselImage
        {
            [ControlFileUpload(LabelText = "Image Url", EnableFineUploader = true)]
            public string ImageUrl { get; set; }

            [ControlText]
            public string Url { get; set; }

            [ControlText]
            public string Title { get; set; }

            [ControlText(LabelText = "Sub Title")]
            public string SubTitle { get; set; }

            [ControlText]
            public string Description { get; set; }
        }
    }
}
