using System.Text;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlCaptchaAttribute : ControlFormAttribute
    {
        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            var value = (CaptchaSettings)Value;

            var sb = new StringBuilder();
            sb.Append("<script type=\"text/javascript\">var RecaptchaOptions = { theme: 'clean' };</script>");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"http://www.google.com/recaptcha/api/challenge?k={0}\"></script>", value.PublicKey);
            sb.Append("<noscript>");
            sb.AppendFormat("<iframe src=\"http://www.google.com/recaptcha/api/noscript?k={0}\" height=\"300\" width=\"500\" frameborder=\"0\"></iframe><br>", value.PublicKey);
            sb.Append("<textarea name=\"recaptcha_challenge_field\" rows=\"3\" cols=\"40\"></textarea>");
            sb.Append("<input type=\"hidden\" name=\"recaptcha_response_field\" value=\"manual_challenge\">");
            sb.Append("</noscript>");

            return sb.ToString();
        }
    }
}