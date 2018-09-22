using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.Collections;
using System.Web.Routing;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlFormAction : ControlFormActionBase<ControlFormAction>
    {
        private readonly IList<string> childActions;

        public ControlFormAction(bool isSubmitButton, bool isValidationSupported)
            : base(isSubmitButton, isValidationSupported)
        {
            childActions = new List<string>();
        }

        public string Value { get; set; }

        public string Url { get; set; }

        public Func<string> HtmlBuilder { get; set; }

        public ControlFormAction HasHtmlBuilder(Func<string> htmlBuilder)
        {
            HtmlBuilder = htmlBuilder;
            return this;
        }

        public ControlFormAction HasValue(string value)
        {
            Value = value;
            return this;
        }

        public ControlFormAction HasUrl(string value)
        {
            IsSubmitButton = false;
            Url = value;
            return this;
        }

        public ControlFormAction AddChildAction(string text)
        {
            return AddChildAction(text, null);
        }

        public ControlFormAction AddChildAction(string text, string href, string onclick = null)
        {
            var tag = new TagBuilder("a") {InnerHtml = text};
            tag.MergeAttribute("href", string.IsNullOrEmpty(href) ? "javascript:void(0);" : href);

            if (!string.IsNullOrEmpty(onclick))
            {
                tag.MergeAttribute("onclick", onclick);
            }

            childActions.Add(string.Format("<li>{0}</li>", tag.ToString(TagRenderMode.Normal)));

            return this;
        }

        public ControlFormAction AddChildDivider()
        {
            childActions.Add("<li class=\"divider\"></li>");
            return this;
        }

        internal virtual string Create(ControlFormProvider formProvider)
        {
            if (HtmlBuilder != null)
            {
                return HtmlBuilder();
            }

            if (childActions.Count > 0)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("<button data-toggle=\"dropdown\" class=\"{0} dropdown-toggle\">", string.IsNullOrEmpty(CssClass) ? "btn" : CssClass.Trim());
                sb.Append(Text);
                sb.Append("&nbsp;<span class=\"caret\"></span>");
                sb.AppendFormat("</button>");

                sb.Append("<ul class=\"dropdown-menu\">");
                foreach (var childAction in childActions)
                {
                    sb.Append(childAction);
                }
                sb.Append("</ul>");

                return sb.ToString();
            }

            if (IsSubmitButton)
            {
                var attributes = new RouteValueDictionary();

                if (!HtmlAttributes.IsNullOrEmpty())
                {
                    foreach (var attribute in HtmlAttributes)
                    {
                        attributes.Add(attribute.Key, attribute.Value);
                    }
                }

                var cssClass = (formProvider.GetButtonSizeCssClass(ButtonSize) + " " + formProvider.GetButtonStyleCssClass(ButtonStyle) + " " + CssClass + (!IsValidationSupported ? " cancel" : "")).Trim();

                if (!string.IsNullOrEmpty(cssClass))
                {
                    attributes.Add("class", cssClass);
                }

                if (!string.IsNullOrEmpty(ClientId))
                {
                    attributes.Add("id", ClientId);
                }

                if (!string.IsNullOrEmpty(ConfirmMessage))
                {
                    attributes.Add("onclick", string.Format("return confirm('{0}');", ConfirmMessage));
                }

                if (!string.IsNullOrEmpty(ClientClickCode))
                {
                    attributes["onclick"] = ClientClickCode;
                }

                var tagBuilder = new TagBuilder("button") { InnerHtml = Text };
                tagBuilder.MergeAttribute("type", "submit");
                tagBuilder.MergeAttribute("value", Value);
                tagBuilder.MergeAttribute("name", Name);
                tagBuilder.MergeAttribute("id", "btn" + Name);
                tagBuilder.MergeAttribute("title", Description ?? Text);
                tagBuilder.MergeAttributes(attributes);

                if (!string.IsNullOrEmpty(IconCssClass))
                {
                    var icon = new TagBuilder("i");
                    icon.AddCssClass(IconCssClass);

                    tagBuilder.InnerHtml = string.Concat(icon.ToString(), " ", Text);
                }

                return tagBuilder.ToString(TagRenderMode.Normal);
            }
            else
            {
                var attributes = new RouteValueDictionary();

                if (!HtmlAttributes.IsNullOrEmpty())
                {
                    foreach (var attribute in HtmlAttributes)
                    {
                        attributes.Add(attribute.Key, attribute.Value);
                    }
                }

                var cssClass = (formProvider.GetButtonSizeCssClass(ButtonSize) + " " + formProvider.GetButtonStyleCssClass(ButtonStyle) + " " + CssClass + (!IsValidationSupported ? " cancel" : "")).Trim();
                if (!string.IsNullOrEmpty(cssClass))
                {
                    attributes.Add("class", cssClass);
                }

                if (!string.IsNullOrEmpty(ClientId))
                {
                    attributes.Add("id", ClientId);
                }

                if (!string.IsNullOrEmpty(ClientClickCode))
                {
                    attributes["onclick"] = ClientClickCode;
                }

                attributes["href"] = Url;

                if (IsShowModalDialog)
                {
                    attributes.Add("data-toggle", "fancybox");
                    attributes.Add("data-fancybox-type", "iframe");
                    attributes.Add("data-fancybox-width", ModalDialogWidth);
                    attributes.Add("data-fancybox-height", ModalDialogHeight);
                }

                var tagBuilder = new TagBuilder("a") { InnerHtml = Text };
                tagBuilder.MergeAttributes(attributes);

                if (!string.IsNullOrEmpty(IconCssClass))
                {
                    var icon = new TagBuilder("i");
                    icon.AddCssClass(IconCssClass);

                    tagBuilder.InnerHtml = string.Concat(icon.ToString(), " ", Text);
                }

                return tagBuilder.ToString(TagRenderMode.Normal);
            }
        }
    }

    public class ControlFormHtmlAction : ControlFormAction
    {
        private readonly Func<string> htmlBuilder;

        public ControlFormHtmlAction(Func<string> htmlBuilder)
            : base(false, false)
        {
            this.htmlBuilder = htmlBuilder;
        }

        internal override string Create(ControlFormProvider formProvider)
        {
            return htmlBuilder();
        }
    }
}