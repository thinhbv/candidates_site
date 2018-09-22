using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using CMSSolutions.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlTextAttribute : ControlFormAttribute
    {
        public ControlText Type { get; set; }

        public int MinLength { get; set; }

        public int MaxLength { get; set; }

        public string EqualTo { get; set; }

        public int Cols { get; set; }

        public int Rows { get; set; }

        public string PlaceHolder { get; set; }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            if (Type == ControlText.RichText)
            {
                yield return ResourceType.RichText;
            }
        }

        public override string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return controlForm.FormProvider.GenerateControlUI(controlForm, this, workContext, htmlHelper);
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            // For security reason, does not show password value
            if (Type == ControlText.Password)
            {
                Value = string.Empty;
            }

            var attributes = new RouteValueDictionary();

            var cssClass = (controlForm.FormProvider.ControlCssClass + " " + CssClass).Trim();
            if (!string.IsNullOrEmpty(cssClass))
            {
                attributes.Add("class", cssClass);
            }

            IHtmlString hiddenText = null;

            if (controlForm.ReadOnly || ReadOnly)
            {
                attributes.Add("readonly", "readonly");
                hiddenText = new MvcHtmlString(string.Format(@"<input type=""hidden"" value=""{0}"" name=""{1}"" />", Value, Name));
            }
            else
            {
                attributes.Add("data-val", "true");

                if (Required || !string.IsNullOrEmpty(RequiredIf))
                {
                    if (!string.IsNullOrEmpty(RequiredIf))
                    {
                        var dependency = RequiredIf.Split(':').First();
                        var requiredIf = RequiredIf.Replace(dependency, Name.Replace(Name.Split('.').Last(), dependency));
                        attributes.Add("data-val-requiredif", "#" + htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(requiredIf.TrimStart('#')));
                    }
                    else
                    {
                        attributes.Add("data-val-required", Constants.Messages.Validation.Required);
                    }
                }

                if (MinLength > 0 && MaxLength > 0)
                {
                    attributes.Add("data-val-length-min", MinLength);
                    attributes.Add("data-val-length-max", MaxLength);
                    attributes.Add("data-val-length", string.Format(Constants.Messages.Validation.RangeLength, MinLength, MaxLength));
                    attributes.Add("maxlength", MaxLength);
                }
                else if (MinLength > 0)
                {
                    attributes.Add("data-val-length-min", MinLength);
                    attributes.Add("data-val-length", string.Format(Constants.Messages.Validation.MinLength, MinLength));
                }
                else if (MaxLength > 0)
                {
                    attributes.Add("data-val-length-max", MaxLength);
                    attributes.Add("data-val-length", string.Format(Constants.Messages.Validation.MaxLength, MaxLength));
                    attributes.Add("maxlength", MaxLength);
                }

                if (!string.IsNullOrEmpty(DataBind))
                {
                    attributes.Add("data-bind", DataBind);
                }

                switch (Type)
                {
                    case ControlText.Email:
                        attributes.Add("data-val-email", Constants.Messages.Validation.Email);
                        attributes["type"] = "email";
                        break;

                    case ControlText.Url:
                        attributes.Add("data-val-url", Constants.Messages.Validation.Url);
                        attributes["type"] = "url";
                        break;

                    case ControlText.Password:
                        attributes["type"] = "password";

                        if (!string.IsNullOrEmpty(EqualTo))
                        {
                            attributes.Add("data-val-equalto", Constants.Messages.Validation.EqualTo);
                            attributes.Add("data-val-equalto-other", EqualTo);
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(PlaceHolder))
                {
                    attributes.Add("placeholder", PlaceHolder);
                }
            }

            string helpText = null;
            if (!string.IsNullOrEmpty(HelpText))
            {
                helpText = string.Format("<span class=\"help-block\">{0}</span>", HelpText);
            }

            var valMsg = string.Format("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", Name);

            switch (Type)
            {
                case ControlText.MultiText:
                    {
                        if (Rows > 0)
                        {
                            attributes.Add("rows", Rows);
                        }
                        else
                        {
                            attributes.Add("rows", "5");
                        }

                        if (Cols > 0)
                        {
                            attributes.Add("cols", Cols);
                        }

                        return string.Join("", htmlHelper.TextArea(Name, (string)Value, attributes), valMsg, helpText);
                    }
                case ControlText.RichText:
                    {
                        if (ReadOnly || controlForm.ReadOnly)
                        {
                            return string.Format("<p>{0}</p>", Value);
                        }

                        attributes["class"] = MergeCssClass(CssClass, "richtext");
                        return string.Join("", htmlHelper.TextArea(Name, (string)Value, attributes), valMsg, helpText);
                    }
                default:
                    {
                        var sb = new StringBuilder();
                        sb.Append(new CombineHtmlString(htmlHelper.TextBox(Name, Value, attributes), hiddenText, valMsg, helpText));

                        if (!string.IsNullOrEmpty(PrependText))
                        {
                            sb.Insert(0, string.Format("<span class=\"help-inline\">{0}</span>", PrependText));
                        }

                        if (!string.IsNullOrEmpty(AppendText))
                        {
                            sb.AppendFormat("<span class=\"help-inline\">{0}</span>", AppendText);
                        }

                        return sb.ToString();
                    }
            }
        }
    }

    public enum ControlText
    {
        TextBox,
        Password,
        Email,
        Url,
        MultiText,
        RichText,
        Image
    }
}