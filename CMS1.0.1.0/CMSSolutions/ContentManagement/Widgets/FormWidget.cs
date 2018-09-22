using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Widgets
{
    [Feature(Constants.Areas.Widgets)]
    public class FormWidget : WidgetBase
    {
        public override string Name
        {
            get { return "Form Widget"; }
        }

        [ControlText(Type = ControlText.RichText, LabelText = "Html Template", ContainerCssClass = "col-xs-9 col-sm-9", ContainerRowIndex = 3)]
        public string HtmlTemplate { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "Enable Captcha", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 4)]
        public bool EnableCaptcha { get; set; }

        [ControlText(Type = ControlText.MultiText, LabelText = "'Thank You' Message", ContainerCssClass = "col-xs-9 col-sm-9", ContainerRowIndex = 5)]
        public string ThankyouMessage { get; set; }

        [ControlText(MaxLength = 255, LabelText = "Redirect Url After Submit", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 6)]
        public string RedirectUrl { get; set; }

        [ControlText(Type = ControlText.Email, Required = true, MaxLength = 255, LabelText = "Email Address", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 6)]
        public string EmailAddress { get; set; }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            yield return ResourceType.JQueryValidate;
        }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            var urlHelper = new UrlHelper(viewContext.RequestContext);

            if (!string.IsNullOrEmpty(CssClass))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (ShowTitleOnPage)
            {
                writer.Write("<header><h3>{0}</h3></header>", Title);
            }

            writer.Write("<form method=\"post\" action=\"{0}\">", urlHelper.Action("Save", "FormWidget", new { area = Constants.Areas.Widgets }));
            writer.Write("<input type=\"hidden\" name=\"EnableCaptcha\" value=\"{0}\"/>", EnableCaptcha);
            writer.Write("<input type=\"hidden\" name=\"ThankyouMessage\" value=\"{0}\"/>", ThankyouMessage);
            writer.Write("<input type=\"hidden\" name=\"RedirectUrl\" value=\"{0}\"/>", RedirectUrl);
            writer.Write("<input type=\"hidden\" name=\"EmailAddress\" value=\"{0}\"/>", EmailAddress);
            writer.Write("<input type=\"hidden\" name=\"WidgetTitle\" value=\"{0}\"/>", Title);
            writer.Write(HtmlTemplate);
            writer.Write("</form>");

            writer.RenderEndTag(); // div
        }
    }
}