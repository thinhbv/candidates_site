using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using CMSSolutions.ContentManagement.Widgets;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.UI;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Newsletters.Widgets
{
    [Feature(Constants.Areas.Newsletters)]
    public class NewsletterSubscribeWidget : WidgetBase
    {
        public override string Name
        {
            get { return "Newsletter Subscribe Widget"; }
        }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            var workContext = workContextAccessor.GetContext();
            var localizer = LocalizationUtilities.Resolve(workContext, typeof(DataAnnotations4ModelMetadataProvider).FullName);

            string html = string.Format(
@"<form id=""newsletter-subscribe-form"" class=""form-horizontal"">
<label for=""email"">{0}</label>
<input type=""text"" id=""newsletter-widget-email"" class=""form-control"" placeholder=""{1}"" />
<input type=""button"" class=""btn btn-primary"" value=""{2}"" onclick=""javascript:newsletterSignUp()"" />
</form>",
        localizer("Sign up for newsletters"),
        localizer("Input e-mail address"),
        localizer("Submit"));

            writer.Write(html);

            var scriptRegister = new ScriptRegister(workContext);
            scriptRegister.IncludeInline(string.Format(
@"function newsletterSignUp() {{
    $.ajax({{
        url: '{0}',
        data: {{ email: $('#newsletter-widget-email').val() }},
        type: 'POST',
        dataType: 'json',
        success: function(result){{
            alert(result.Message);
        }}
    }});
}}",
    new UrlHelper(viewContext.RequestContext).Action("Subscribe", "Subscription", new { area = Constants.Areas.Newsletters })));
        }
    }
}
