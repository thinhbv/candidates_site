using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Net.Mail;
using CMSSolutions.Web;
using CMSSolutions.Web.Mvc;

namespace CMSSolutions.ContentManagement.Widgets.Controllers
{
    [Authorize]
    [Feature(Constants.Areas.Widgets)]
    public class FormWidgetController : BaseController
    {
        private readonly CaptchaSettings captchaSettings;
        private readonly IEmailSender emailSender;

        public FormWidgetController(IWorkContextAccessor workContextAccessor,
            CaptchaSettings captchaSettings, IEmailSender emailSender)
            : base(workContextAccessor)
        {
            this.captchaSettings = captchaSettings;
            this.emailSender = emailSender;
        }

        [HttpPost, Url("form-widget/save")]
        public ActionResult Save(bool enableCaptcha, string thankyouMessage, string redirectUrl, string emailAddress, string widgetTitle)
        {
            // Validate captcha
            if (enableCaptcha)
            {
                var captchaChallengeValue = Request.Form["recaptcha_challenge_field"];
                var captchaResponseValue = Request.Form["recaptcha_response_field"];

                if (!string.IsNullOrEmpty(captchaChallengeValue))
                {
                    if (string.IsNullOrEmpty(captchaResponseValue))
                    {
                        throw new ArgumentException(RecaptchaResponse.InvalidResponse.ErrorMessage);
                    }

                    var captchaValidator = new RecaptchaValidator
                    {
                        PrivateKey = captchaSettings.PrivateKey,
                        RemoteIP = Request.UserHostAddress,
                        Challenge = captchaChallengeValue,
                        Response = captchaResponseValue
                    };

                    var validateResult = captchaValidator.Validate();
                    if (!validateResult.IsValid)
                    {
                        throw new ArgumentException(validateResult.ErrorMessage);
                    }
                }
            }

            var values = Request.Form.AllKeys.ToDictionary(key => key, key => (object)Request.Form[key]);

            // Remove some items
            values.Remove("EnableCaptcha");
            values.Remove("ThankyouMessage");
            values.Remove("RedirectUrl");
            values.Remove("EmailAddress");
            values.Remove("WidgetTitle");
            values.Remove("X-Requested-With");

            var subject = widgetTitle;
            var body = new StringBuilder();
            body.Append(subject);
            body.Append("<br/>");

            body.Append("<table style=\"width: 100%; border-collapse: collapse; border-spacing: 0;\">");

            foreach (var value in values)
            {
                body.Append("<tr>");

                body.Append("<td style=\"border-color: #DDDDDD; border-style: solid; border-width: 1px; color: #000000; font-size: 12px; padding: 7px;\">");
                body.Append(value.Key);
                body.Append("</td>");

                body.Append("<td style=\"border-color: #DDDDDD; border-style: solid; border-width: 1px; color: #000000; font-size: 12px; padding: 7px;\">");
                body.Append(value.Value);
                body.Append("</td>");
            }

            body.Append("</table>");

            emailSender.Send(subject, body.ToString(), emailAddress);

            var result = new AjaxResult();

            if (!string.IsNullOrEmpty(thankyouMessage))
            {
                result.Alert(thankyouMessage);
            }

            result.Redirect(!string.IsNullOrWhiteSpace(redirectUrl) ? redirectUrl : Url.Content("~/"));

            return result;
        }
    }
}