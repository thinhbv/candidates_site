using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using CMSSolutions.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Routing;

namespace CMSSolutions.Web.UI.ControlForms
{
    public enum ControlChoice
    {
        CheckBox,
        CheckBoxList,
        DropDownList,
        RadioButtonList,
    }

    public class ControlChoiceAttribute : ControlFormAttribute
    {
        private readonly ControlChoice type;

        public ControlChoiceAttribute(ControlChoice type)
        {
            this.type = type;
            RenderLabelControl = true;
        }

        public bool AllowMultiple { get; set; }

        public int Columns { get; set; }

        public bool EnableChosen { get; set; }

        public string ChosenTextDefault { get; set; }

        public bool GroupedByCategory { get; set; }

        public override bool HasLabelControl
        {
            get { return RenderLabelControl; }
        }

        public string OnSelectedIndexChanged { get; set; }

        public string OptionLabel { get; set; }

        public bool RenderLabelControl { get; set; }

        public bool RequiredIfHaveItemsOnly { get; set; }

        public IEnumerable<SelectListItem> SelectListItems { get; set; }

        public ControlChoice Type { get { return type; } }

        public override string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return controlForm.FormProvider.GenerateControlUI(controlForm, this, workContext, htmlHelper);
        }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            if (EnableChosen)
            {
                yield return ResourceType.Chosen;
            }
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            switch (type)
            {
                case ControlChoice.CheckBox: return GenerateCheckBox(controlForm, htmlHelper);
                case ControlChoice.CheckBoxList: return GenerateCheckBoxList(controlForm, htmlHelper);
                case ControlChoice.DropDownList:
                case ControlChoice.RadioButtonList: return GenerateSingleChoice(controlForm, htmlHelper);
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetEnumValueDescription(Type type, object value)
        {
            var field = type.GetField(Convert.ToString(value));
            var attrs = field.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attrs.Length > 0)
            {
                return ((DisplayAttribute)attrs[0]).GetName();
            }

            return Convert.ToString(value);
        }

        private string GenerateCheckBox<TModel>(ControlFormResult<TModel> controlForm, HtmlHelper htmlHelper) where TModel : class
        {
            if (PropertyType != typeof(bool) && PropertyType != typeof(bool?))
            {
                throw new NotSupportedException("Cannot apply control choice for non-Boolean property as checkbox.");
            }

            var attributes = new RouteValueDictionary();

            if (Required)
            {
                attributes.Add("data-val", "true");
                attributes.Add("data-val-required", Constants.Messages.Validation.Required);
            }

            if (!string.IsNullOrEmpty(DataBind))
            {
                attributes.Add("data-bind", DataBind);
            }

            if (!string.IsNullOrEmpty(OnSelectedIndexChanged))
            {
                attributes.Add("onchange", OnSelectedIndexChanged);
            }

            if (ReadOnly || controlForm.ReadOnly)
            {
                attributes.Add("disabled", "disabled");
            }

            var sbCheckBox = new StringBuilder();

            sbCheckBox.AppendFormat("<div class=\"checkbox\"><label class=\"{0}\">", CssClass);

            if (!string.IsNullOrEmpty(PrependText))
            {
                sbCheckBox.Append(PrependText);
                sbCheckBox.Append("&nbsp;");
            }

            var checkBox = htmlHelper.CheckBox(Name, Convert.ToBoolean(Value), attributes);
            sbCheckBox.Append(checkBox);

            if (!string.IsNullOrEmpty(AppendText))
            {
                sbCheckBox.Append("&nbsp;");
                sbCheckBox.Append(AppendText);
            }
         
            sbCheckBox.Append("</label></div>");

            if (!string.IsNullOrEmpty(HelpText))
            {
                sbCheckBox.AppendFormat("<span class=\"help-block\">{0}</span>", HelpText);
            }

            return sbCheckBox.ToString();
        }

        private string GenerateCheckBoxList<TModel>(ControlFormResult<TModel> controlForm, HtmlHelper htmlHelper) where TModel : class
        {
            if (!typeof(IEnumerable).IsAssignableFrom(PropertyType))
            {
                throw new NotSupportedException("Cannot apply control choice for non enumerable property as checkbox list.");
            }

            var value = Value as IEnumerable;
            var values = new List<string>();
            if (value != null)
            {
                values.AddRange(from object item in value select Convert.ToString(item));
            }

            IList<SelectListItem> selectItems;

            if (SelectListItems == null)
            {
                selectItems = controlForm.GetExternalDataSource(Name.RemoveBetween('[', ']'));

                if (selectItems == null)
                {
                    throw new NotSupportedException("You need to register an external data source for " + Name);
                }
            }
            else
            {
                selectItems = SelectListItems.ToList();
            }

            var cssClass = CssClass ?? "checkbox";

            var index = 0;

            var sb = new StringBuilder();

            if (GroupedByCategory)
            {
                var items = selectItems.Cast<ExtendedSelectListItem>().ToList();
                var groups = items.GroupBy(x => x.Category);

                foreach (var @group in groups)
                {
                    sb.AppendFormat("<strong>{0}</strong>", group.Key);

                    foreach (var item in group)
                    {
                        var isChecked = values.Contains(item.Value);
                        var checkbox = new TagBuilder("input");

                        string name = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(Name);
                        checkbox.MergeAttribute("type", "checkbox");
                        checkbox.MergeAttribute("name", name);
                        checkbox.MergeAttribute("id", htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(Name) + "_" + index);
                        checkbox.MergeAttribute("value", item.Value);
                        if (isChecked)
                        {
                            checkbox.MergeAttribute("checked", "checked");
                        }

                        if (ReadOnly || controlForm.ReadOnly)
                        {
                            checkbox.MergeAttribute("disabled", "disabled");
                        }

                        sb.AppendFormat("<label for=\"{3}\" class=\"{2}\">{1}{0}</label>", item.Text, checkbox.ToString(TagRenderMode.SelfClosing), cssClass, name);
                        index++;
                    }
                }
            }
            else
            {
                var columns = (Columns > 0) ? Columns : 1;
                var rows = (int)Math.Ceiling((selectItems.Count() * 1d) / columns);
                var columnWidth = (int)Math.Ceiling(12d / columns);

                for (var i = 0; i < columns; i++)
                {
                    var items = selectItems.Skip(i * rows).Take(rows);
                    sb.AppendFormat("<div class=\"span{0}\">", columnWidth);

                    foreach (var item in items)
                    {
                        var isChecked = values.Contains(item.Value);
                        var checkbox = new TagBuilder("input");
                        checkbox.MergeAttribute("type", "checkbox");
                        checkbox.MergeAttribute("name", htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(Name));
                        checkbox.MergeAttribute("id", htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(Name) + "_" + index);
                        checkbox.MergeAttribute("value", item.Value);
                        if (isChecked)
                        {
                            checkbox.MergeAttribute("checked", "checked");
                        }

                        if (ReadOnly || controlForm.ReadOnly)
                        {
                            checkbox.MergeAttribute("disabled", "disabled");
                        }

                        sb.AppendFormat("<label class=\"{2}\">{1}{0}</label>", item.Text, checkbox.ToString(TagRenderMode.SelfClosing), cssClass);
                        index++;
                    }

                    sb.Append("</div>");
                }
            }

            return sb.ToString();
        }

        private string GenerateSingleChoice<TModel>(ControlFormResult<TModel> controlForm, HtmlHelper htmlHelper) where TModel : class
        {
            var attributes = new RouteValueDictionary();
            var valMsg = new MvcHtmlString(string.Format("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", Name));

            var selectedValue = Convert.ToString(Value);

            IList<SelectListItem> selectItems;
            if (PropertyType.IsEnum)
            {
                var values = Enum.GetValues(PropertyType);

                selectItems = (from object value in values
                               select new SelectListItem
                               {
                                   Text = GetEnumValueDescription(PropertyType, value),
                                   Value = Convert.ToString(value),
                               }).ToList();
            }
            else
            {
                if (SelectListItems == null)
                {
                    var tmpSelectItems = controlForm.GetExternalDataSource(Name);

                    if (tmpSelectItems == null && Name.Contains('['))
                    {
                        tmpSelectItems = controlForm.GetExternalDataSource(Name.RemoveBetween('[', ']'));
                    }

                    if (tmpSelectItems == null && Name.Contains('.'))
                    {
                        tmpSelectItems = controlForm.GetExternalDataSource((Name + ".").RemoveBetween('.', '.'));
                    }

                    if (tmpSelectItems == null)
                    {
                        throw new NotSupportedException("You need to register an external data source for " + Name);
                    }
                    selectItems = tmpSelectItems.ToList();
                }
                else
                {
                    selectItems = SelectListItems.ToList();
                }
            }

            var cssClass = (controlForm.FormProvider.ControlCssClass + " " + CssClass).Trim();
            if (EnableChosen)
            {
                cssClass = CssClass;
            }
            if (!string.IsNullOrEmpty(cssClass))
            {
                attributes.Add("class", cssClass);
            }

            if (ReadOnly || controlForm.ReadOnly)
            {
                string selectedText = null;

                var item = selectItems.FirstOrDefault(x => x.Value == selectedValue);
                if (item != null)
                {
                    selectedText = item.Text;
                }

                return string.Format(
                    @"<input type=""hidden"" id=""{3}"" name=""{0}"" value=""{1}"" /><input type=""text"" class=""{4}"" readonly=""readonly"" value=""{2}"" />",
                    Name,
                    selectedValue,
                    selectedText,
                    htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(Name),
                    cssClass);
            }

            if (Required || !string.IsNullOrEmpty(RequiredIf))
            {
                attributes.Add("data-val", "true");
                if (!string.IsNullOrEmpty(RequiredIf))
                {
                    if (Name.Contains("."))
                    {
                        var dependency = RequiredIf.TrimStart('#').Split(':').First();
                        var requiredIf = RequiredIf.Replace(dependency, Name.Replace(Name.Split('.').Last(), dependency));
                        attributes.Add("data-val-requiredif", "#" + htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(requiredIf.TrimStart('#')));
                    }
                    else
                    {
                        attributes.Add("data-val-requiredif", "#" + RequiredIf.TrimStart('#'));
                    }
                }
                else
                {
                    if (RequiredIfHaveItemsOnly == false || selectItems.Any())
                    {
                        attributes.Add("data-val-required", Constants.Messages.Validation.Required);
                    }
                }
            }

            if (!string.IsNullOrEmpty(DataBind))
            {
                attributes["data-bind"] = DataBind;
            }

            if (type == ControlChoice.DropDownList)
            {
                var builder = new StringBuilder();

                if (!string.IsNullOrEmpty(OnSelectedIndexChanged))
                {
                    attributes["onchange"] = OnSelectedIndexChanged;
                }

                var selectedValues = new List<string>();

                if (AllowMultiple)
                {
                    attributes["multiple"] = "multiple";

                    var value = Value as IEnumerable;
                    if (value != null)
                    {
                        selectedValues.AddRange(from object item in value select Convert.ToString(item));
                    }
                }

                var selectTag = htmlHelper.DropDownList(Name, new SelectListItem[] { }, attributes);

                if (!Required)
                {
                    builder.AppendLine("<option>" + OptionLabel + "</option>");
                }

                if (GroupedByCategory && EnableChosen)
                {
                    var items = selectItems.Cast<ExtendedSelectListItem>().ToList();
                    var groups = items.GroupBy(x => x.Category);

                    foreach (var @group in groups)
                    {
                        builder.AppendFormat("<optgroup label=\"{0}\">", group.Key);

                        foreach (var item in group)
                        {
                            var optionTag = new TagBuilder("option") { InnerHtml = HttpUtility.HtmlEncode(item.Text) };
                            if (item.Value != null)
                            {
                                optionTag.Attributes["value"] = item.Value;
                            }

                            if (AllowMultiple)
                            {
                                if (selectedValues.Contains(item.Value))
                                {
                                    optionTag.Attributes["selected"] = "selected";
                                }
                            }
                            else
                            {
                                if (item.Value == selectedValue)
                                {
                                    optionTag.Attributes["selected"] = "selected";
                                }
                            }

                            if (item != null && item.HtmlAttributes != null)
                            {
                                var htmlAttributes = item.HtmlAttributes as IDictionary<string, object>;
                                optionTag.MergeAttributes(htmlAttributes ?? HtmlHelper.AnonymousObjectToHtmlAttributes(item.HtmlAttributes));
                            }

                            builder.AppendLine(optionTag.ToString(TagRenderMode.Normal));
                        }

                        builder.Append("</optgroup>");
                    }
                }
                else
                {
                    foreach (var selectItem in selectItems)
                    {
                        var optionTag = new TagBuilder("option") { InnerHtml = HttpUtility.HtmlEncode(selectItem.Text) };
                        if (selectItem.Value != null)
                        {
                            optionTag.Attributes["value"] = selectItem.Value;
                        }

                        if (AllowMultiple)
                        {
                            if (selectedValues.Contains(selectItem.Value))
                            {
                                optionTag.Attributes["selected"] = "selected";
                            }
                        }
                        else
                        {
                            if (selectItem.Value == selectedValue)
                            {
                                optionTag.Attributes["selected"] = "selected";
                            }
                        }

                        var extendedSelectListItem = selectItem as ExtendedSelectListItem;
                        if (extendedSelectListItem != null && extendedSelectListItem.HtmlAttributes != null)
                        {
                            var htmlAttributes = extendedSelectListItem.HtmlAttributes as IDictionary<string, object>;
                            optionTag.MergeAttributes(htmlAttributes ?? HtmlHelper.AnonymousObjectToHtmlAttributes(extendedSelectListItem.HtmlAttributes));
                        }

                        builder.AppendLine(optionTag.ToString(TagRenderMode.Normal));
                    }
                }

                var html = selectTag.ToHtmlString();

                builder.Insert(0, html.Replace("</select>", ""));
                builder.Append("</select>");

                if (!string.IsNullOrEmpty(PrependText) && !string.IsNullOrEmpty(AppendText))
                {
                    builder.Insert(0, string.Format("<div class=\"input-prepend input-append\"><span class=\"add-on\">{0}</span>", PrependText));
                    builder.AppendFormat("<span class=\"add-on\">{0}</span>", AppendText);
                    builder.Append("</div>");
                }

                else if (!string.IsNullOrEmpty(PrependText))
                {
                    builder.Insert(0, string.Format("<div class=\"input-prepend\"><span class=\"add-on\">{0}</span>", PrependText));
                    builder.Append("</div>");
                }

                else if (!string.IsNullOrEmpty(AppendText))
                {
                    builder.Insert(0, "<div class=\"input-append\">");
                    builder.AppendFormat("<span class=\"add-on\">{0}</span>", AppendText);
                    builder.Append("</div>");
                }

                builder.Append(valMsg);

                if (!string.IsNullOrEmpty(HelpText))
                {
                    builder.AppendFormat("<span class=\"help-block\">{0}</span>", HelpText);
                }

                #region Scripts

                if (EnableChosen)
                {
                    builder.Append("<script type=\"text/javascript\">");
                    builder.Append("$(document).ready(function(){");
                    builder.AppendFormat("$('#{0}').chosen({{ no_results_text: \"No results matched\", allow_single_deselect:true, inherit_select_classes: true, display_selected_options: true }});", htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(Name));
                    builder.Append("});");
                    builder.Append("</script>");
                }

                #endregion Scripts

                return builder.ToString();
            }

            #region Radio Buttons

            cssClass = CssClass ?? "radio";
            attributes.Remove("class");

            var sb = new StringBuilder();
            var index = 0;
            foreach (var selectItem in selectItems)
            {
                string dataBind = null;
                var extendedSelectListItem = selectItem as ExtendedSelectListItem;
                if (extendedSelectListItem != null && extendedSelectListItem.HtmlAttributes != null)
                {
                    attributes = attributes.Merge(extendedSelectListItem.HtmlAttributes);
                    if (attributes.ContainsKey("container-data-bind"))
                    {
                        dataBind = Convert.ToString(attributes["container-data-bind"]);
                        attributes.Remove("container-data-bind");
                    }
                }

                attributes["id"] = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(Name + "_" + index);

                var radioButton = htmlHelper.RadioButton(Name, selectItem.Value, selectItem.Value == selectedValue, attributes);
                sb.AppendFormat("<div class=\"{2}\" data-bind=\"{3}\"><label>{1}{0}</label></div>", selectItem.Text, radioButton, cssClass, dataBind);
                index++;
            }

            return sb.ToString();

            #endregion Radio Buttons
        }
    }
}