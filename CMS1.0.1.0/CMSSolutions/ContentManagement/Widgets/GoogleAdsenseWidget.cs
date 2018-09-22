using System.Web.Mvc;
using System.Web.UI;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Widgets
{
    [Feature(Constants.Areas.Widgets)]
    public class GoogleAdsenseWidget : WidgetBase
    {
        public override string Name
        {
            get { return "Google Adsense Widget"; }
        }

        [ControlText(Required = true, LabelText = "Google Ad Client", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public string AdClient { get; set; }

        [ControlText(Required = true, LabelText = "Google Ad Slot", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public string AdSlot { get; set; }

        [ControlNumeric(Required = true, LabelText = "Google Ad Width", MinimumValue = "0", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public int Width { get; set; }

        [ControlNumeric(Required = true, LabelText = "Google Ad Height", MinimumValue = "0", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 4)]
        public int Height { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "Enable LazyLoad Ad", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 4)]
        public bool EnableLazyLoadAd { get; set; }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            if (string.IsNullOrEmpty(AdClient))
            {
                return;
            }

            if (!string.IsNullOrEmpty(CssClass))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
            }

            if (EnableLazyLoadAd)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "lazyload_ad");
                writer.AddAttribute("original", "http://pagead2.googlesyndication.com/pagead/show_ads.js");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
                writer.RenderBeginTag(HtmlTextWriterTag.Code);
                writer.WriteLine("<!--");
                writer.WriteLine("google_ad_client = \"{0}\";", AdClient);
                writer.WriteLine("google_ad_slot = \"{0}\";", AdSlot);
                writer.WriteLine("google_ad_width = {0};", Width);
                writer.WriteLine("google_ad_height = {0};", Height);
                writer.WriteLine("//-->");
                writer.RenderEndTag(); // code
                
                writer.RenderEndTag(); // div
            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
                writer.RenderBeginTag(HtmlTextWriterTag.Script);
                writer.WriteLine();
                writer.WriteLine("google_ad_client = \"{0}\";", AdClient);
                writer.WriteLine("google_ad_slot = \"{0}\";", AdSlot);
                writer.WriteLine("google_ad_width = {0};", Width);
                writer.WriteLine("google_ad_height = {0};", Height);
                writer.RenderEndTag(); // script

                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
                writer.AddAttribute(HtmlTextWriterAttribute.Src, "http://pagead2.googlesyndication.com/pagead/show_ads.js");
                writer.RenderBeginTag(HtmlTextWriterTag.Script);
                writer.RenderEndTag(); // script
            }

            if (!string.IsNullOrEmpty(CssClass))
            {
                writer.RenderEndTag(); // div
            }
        }
    }
}