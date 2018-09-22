﻿using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using CMSSolutions.ContentManagement.Widgets;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization.Services;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Localization
{
    [Feature(Constants.Areas.Localization)]
    public class LanguageSwitchWidget : WidgetBase
    {
        public override string Name
        {
            get { return "Language Switch"; }
        }

        [ControlText(Type = ControlText.TextBox, MaxLength = 255, LabelText = "Message Text", ContainerCssClass = "col-xs-6 col-sm-6", ContainerRowIndex = 3)]
        public string MessageText { get; set; }

        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Style", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public LanguageSwitchStyle Style { get; set; }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            var workContext = workContextAccessor.GetContext(viewContext.HttpContext);
            var languageService = workContext.Resolve<ILanguageService>();

            var languages = languageService.GetActiveLanguages().ToList();
            if (languages.Count() < 2)
            {
                return;
            }

            var currentCulture = workContext.CurrentCulture;

            writer.Write("<div class=\"language-switch hidden-xs language-switch-{0}\">", Style.ToString().ToLowerInvariant());

            switch (Style)
            {
                case LanguageSwitchStyle.Select:
                    if (!string.IsNullOrEmpty(MessageText))
                    {
                        writer.Write("<div class=\"input-prepend\">");
                        writer.Write("<span class=\"add-on\">{0}</span>", MessageText);
                    }

                    writer.Write("<select onchange=\"document.cookie = 'CurrentCulture=; expires=Sat, 1 Jan 1970 17:00:00 GMT; path=/'; document.cookie = 'CurrentCulture=' + this.value + '; path=/'; window.location = window.location;\" autocomplete=\"off\">");

                    foreach (var language in languages)
                    {
                        writer.Write(
                            language.CultureCode == currentCulture
                                ? "<option value=\"{0}\" selected=\"selected\">{1}</option>"
                                : "<option value=\"{0}\">{1}</option>", language.CultureCode, language.Name);
                    }

                    writer.Write("</select>");

                    if (!string.IsNullOrEmpty(MessageText))
                    {
                        writer.Write("</div>");
                    }
                break;
                    case LanguageSwitchStyle.List:
                    writer.RenderBeginTag(HtmlTextWriterTag.Ul);
                    foreach (var language in languages)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, language.CultureCode);
                        writer.RenderBeginTag(HtmlTextWriterTag.Li);

                        writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void(0)");
                        writer.AddAttribute(HtmlTextWriterAttribute.Onclick, string.Format("document.cookie = 'CurrentCulture=; expires=Sat, 1 Jan 1970 17:00:00 GMT; path=/'; document.cookie = 'CurrentCulture={0}; path=/'; window.location = window.location;", language.CultureCode));
                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        writer.Write("<img src='/Images/flags/{0}.png'> {1}", language.CultureCode, language.Name);
                        writer.RenderEndTag(); // a
                        writer.RenderEndTag(); // li
                    }
                    writer.RenderEndTag(); // ul
                break;
            }

            writer.Write("</div>");
        }

        public enum LanguageSwitchStyle
        {
            Select,
            List
        }
    }
}