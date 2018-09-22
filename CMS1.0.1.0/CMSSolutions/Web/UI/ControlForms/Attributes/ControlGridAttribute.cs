using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlGridAttribute : ControlFormAttribute
    {
        private List<ControlFormAttribute> attributes;
        private readonly byte defaultRows;

        public bool ShowLabelControl { get; set; }

        public override bool HasLabelControl
        {
            get { return ShowLabelControl; }
        }

        public byte MinRows { get; set; }

        public byte MaxRows { get; set; }

        public bool ShowAsStack { get; set; }

        public bool ShowTableHead { get; set; }

        public string TableHeadHtml { get; set; }

        public bool ShowRowsControl { get; set; }

        public bool EnabledScroll { get; set; }

        public ControlGridAttribute()
        {
            ShowTableHead = true;
            EnabledScroll = false;
        }

        public ControlGridAttribute(byte minRows, byte maxRows)
            : this()
        {
            ShowRowsControl = true;
            defaultRows = minRows;
            MinRows = minRows;
            MaxRows = maxRows;
        }

        public ControlGridAttribute(byte minRows, byte maxRows, byte defaultRows)
            : this()
        {
            if (maxRows < minRows || maxRows == 0)
            {
                throw new ArgumentOutOfRangeException("maxRows");
            }

            if (defaultRows < minRows || defaultRows > maxRows)
            {
                throw new ArgumentOutOfRangeException("defaultRows");
            }

            ShowRowsControl = true;
            this.defaultRows = defaultRows;
            MinRows = minRows;
            MaxRows = maxRows;
        }

        public override string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return controlForm.FormProvider.GenerateControlUI(controlForm, this, workContext, htmlHelper);
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            EnsureProperties();
            var clientId = "tbl_" + Guid.NewGuid().ToString("N").ToLowerInvariant();

            var sb = new StringBuilder();

            sb.Append("<div class=\"box\">");

            sb.AppendFormat("<div class=\"box-header\"><div class=\"title\">{0}</div></div><div class=\"box-content nopadding\">", LabelText);

            // Fake Index value
            sb.AppendFormat("<input type=\"hidden\" name=\"{0}.Index\" value=\"\" />", Name);

            sb.AppendFormat("<table id=\"{0}\" class=\"{1}\" data-min-rows=\"{2}\">", clientId, CssClass, MinRows);

            if (!ShowAsStack && (ShowTableHead || !string.IsNullOrEmpty(TableHeadHtml)))
            {
                sb.Append("<thead>");

                if (string.IsNullOrEmpty(TableHeadHtml))
                {
                    sb.Append("<tr>");

                    sb.Append("<th style=\"display: none;\"></th>");

                    foreach (var controlAttribute in attributes)
                    {
                        if (controlAttribute is ControlHiddenAttribute)
                        {
                            sb.Append("<th style=\"display: none;\">&nbsp;</th>");
                        }
                        else
                        {
                            if (controlAttribute.ColumnWidth > 0)
                            {
                                sb.AppendFormat("<th style=\"width: {1}px;\">{0}</th>", controlAttribute.LabelText, controlAttribute.ColumnWidth);
                            }
                            else
                            {
                                sb.AppendFormat("<th>{0}</th>", controlAttribute.LabelText);
                            }
                        }
                    }

                    if (ShowRowsControl)
                    {
                        if (!ReadOnly && !controlForm.ReadOnly)
                        {
                            sb.AppendFormat("<th style=\"width: 1%;\">&nbsp;</th>");
                        }
                    }

                    sb.Append("</tr>");
                }
                else
                {
                    sb.Append(TableHeadHtml);
                }
                sb.Append("</thead>");
            }

            var actualRows = 0;

            var value = Value as IEnumerable<object>;
            if (value != null)
            {
                actualRows = value.Count();
            }

            var maxRows = ShowRowsControl ? MaxRows : actualRows;

            sb.Append("<tbody>");

            for (var i = 0; i < maxRows; i++)
            {
                if (i >= actualRows && (i >= defaultRows || actualRows > 0))
                {
                    sb.Append("<tr style=\"display: none;\">");
                    sb.AppendFormat("<td style=\"display: none;\"><input type=\"checkbox\" name=\"{0}.Index\" class=\"ControlGrid__Index\" value=\"{1}\" autocomplete=\"off\" /></td>", Name, i);
                }
                else
                {
                    sb.Append("<tr>");
                    sb.AppendFormat("<td style=\"display: none;\"><input type=\"checkbox\" name=\"{0}.Index\" class=\"ControlGrid__Index\" value=\"{1}\" checked=\"checked\" autocomplete=\"off\" /></td>", Name, i);
                }

                if (ShowAsStack)
                {
                    sb.Append("<td style=\"padding: 0;\">");

                    foreach (var controlAttribute in attributes)
                    {
                        if (!string.IsNullOrEmpty(controlAttribute.ControlSpan))
                        {
                            continue;
                        }

                        controlAttribute.Name = Name + "[" + i + "]." + controlAttribute.PropertyName;

                        if (value != null && i < actualRows)
                        {
                            var obj = value.ElementAt(i);
                            controlAttribute.Value = controlForm.GetPropertyValue(obj, controlAttribute.PropertyName);
                        }
                        else
                        {
                            controlAttribute.Value = GetDefaultValue(controlAttribute.PropertyType);
                        }

                        if (controlAttribute is ControlHiddenAttribute)
                        {
                            sb.Append(controlAttribute.GenerateControlUI(controlForm, workContext, htmlHelper));
                        }
                        else
                        {
                            var propertyName = controlAttribute.PropertyName;
                            var spanAttributes = attributes.Where(x => x.ControlSpan == propertyName).ToList();

                            foreach (var spanAttribute in spanAttributes)
                            {
                                spanAttribute.Name = Name + "[" + i + "]." + spanAttribute.PropertyName;

                                if (value != null && i < actualRows)
                                {
                                    var obj = value.ElementAt(i);
                                    spanAttribute.Value = controlForm.GetPropertyValue(obj, spanAttribute.PropertyName);
                                }
                                else
                                {
                                    spanAttribute.Value = GetDefaultValue(spanAttribute.PropertyType);
                                }
                            }

                            var spanControls = spanAttributes.Select(x => x.GenerateControlUI(controlForm, workContext, htmlHelper)).ToList();
                            spanControls.Insert(0, controlAttribute.GenerateControlUI(controlForm, workContext, htmlHelper));

                            if (controlAttribute.HasLabelControl)
                            {
                                sb.AppendFormat("<div class=\"control-group\"><label class=\"control-label\">{0}</label>", controlAttribute.LabelText);
                                sb.AppendFormat("<div class=\"controls\">{0}</div></div>", string.Join("&nbsp;&nbsp;&nbsp;", spanControls));
                            }
                            else
                            {
                                sb.AppendFormat("<div class=\"control-group\"><div class=\"controls\">{0}</div></div>", string.Join("&nbsp;&nbsp;&nbsp;", spanControls));
                            }
                        }
                    }

                    sb.Append("</td>");
                }
                else
                {
                    foreach (var controlAttribute in attributes)
                    {
                        controlAttribute.Name = Name + "[" + i + "]." + controlAttribute.PropertyName;

                        if (value != null && i < actualRows)
                        {
                            var obj = value.ElementAt(i);
                            controlAttribute.Value = controlForm.GetPropertyValue(obj, controlAttribute.PropertyName);
                        }
                        else
                        {
                            controlAttribute.Value = GetDefaultValue(controlAttribute.PropertyType);
                        }

                        if (ReadOnly)
                        {
                            if (controlAttribute is ControlHiddenAttribute)
                            {
                                sb.AppendFormat("<td style=\"display: none;\">{0}</td>", controlAttribute.Value);
                            }
                            else
                            {
                                sb.AppendFormat("<td>{0}</td>", controlAttribute.Value);
                            }
                        }
                        else
                        {
                            if (controlAttribute is ControlHiddenAttribute)
                            {
                                sb.AppendFormat("<td style=\"display: none;\">{0}</td>", controlAttribute.GenerateControlUI(controlForm, workContext, htmlHelper));
                            }
                            else
                            {
                                sb.AppendFormat("<td>{0}</td>", controlAttribute.GenerateControlUI(controlForm, workContext, htmlHelper));
                            }
                        }
                    }
                }

                if (ShowRowsControl)
                {
                    if (!ReadOnly && !controlForm.ReadOnly)
                    {
                        sb.AppendFormat("<td style=\"width: 1%; vertical-align: top;\"><button type=\"button\" onclick=\"var visible = $('#{0} tbody tr:visible').length; var min = parseInt($('#{0}').data('min-rows')); if(visible >= min) {{ $(this).closest('tr').hide().find('.ControlGrid__Index').removeAttr('checked'); $('#{0}_AddButton').show(); }}\" class=\"{1} {2} pull-right\"><i class=\"fa fa-times\"></i></button></td>", clientId, controlForm.FormProvider.GetButtonSizeCssClass(ButtonSize.ExtraSmall), controlForm.FormProvider.GetButtonStyleCssClass(ButtonStyle.Danger));
                    }
                }

                sb.Append("</tr>");
            }
            sb.Append("</tbody>");

            if (ShowRowsControl)
            {
                if (!ReadOnly && !controlForm.ReadOnly)
                {
                    if (ShowAsStack)
                    {
                        if (actualRows == maxRows || defaultRows == maxRows)
                        {
                            sb.AppendFormat("<tfoot><tr><td colspan=\"{0}\"><button style=\"display:none;\" id=\"{1}_AddButton\" type=\"button\" onclick=\"$('#{1} tbody tr:hidden').first().show().find('.ControlGrid__Index').attr('checked','checked'); var hidden = $('#{1} tbody tr:hidden').length; if(hidden == 0){{ $('#{1}_AddButton').hide(); }}\" class=\"{2} {3} pull-right\"><i class=\"fa fa-plus-square\"></i></button></td></tr></tfoot>", 3, clientId, controlForm.FormProvider.GetButtonSizeCssClass(ButtonSize.ExtraSmall), controlForm.FormProvider.GetButtonStyleCssClass(ButtonStyle.Info));
                        }
                        else
                        {
                            sb.AppendFormat("<tfoot><tr><td colspan=\"{0}\"><button id=\"{1}_AddButton\" type=\"button\" onclick=\"$('#{1} tbody tr:hidden').first().show().find('.ControlGrid__Index').attr('checked','checked'); var hidden = $('#{1} tbody tr:hidden').length; if(hidden == 0){{ $('#{1}_AddButton').hide(); }}\" class=\"{2} {3} pull-right\"><i class=\"fa fa-plus-square\"></i></button></td></tr></tfoot>", 3, clientId, controlForm.FormProvider.GetButtonSizeCssClass(ButtonSize.ExtraSmall), controlForm.FormProvider.GetButtonStyleCssClass(ButtonStyle.Success));
                        }
                    }
                    else
                    {
                        if (actualRows == maxRows || defaultRows == maxRows)
                        {
                            sb.AppendFormat("<tfoot><tr><td colspan=\"{0}\"><button style=\"display:none;\" id=\"{1}_AddButton\" type=\"button\" onclick=\"$('#{1} tbody tr:hidden').first().show().find('.ControlGrid__Index').attr('checked','checked'); var hidden = $('#{1} tbody tr:hidden').length; if(hidden == 0){{ $('#{1}_AddButton').hide(); }}\" class=\"{2} {3} pull-right\"><i class=\"fa fa-plus-square\"></i></button></td></tr></tfoot>", attributes.Count + 1, clientId, controlForm.FormProvider.GetButtonSizeCssClass(ButtonSize.ExtraSmall), controlForm.FormProvider.GetButtonStyleCssClass(ButtonStyle.Success));
                        }
                        else
                        {
                            sb.AppendFormat("<tfoot><tr><td colspan=\"{0}\"><button id=\"{1}_AddButton\" type=\"button\" onclick=\"var row = $('#{1} tbody tr:hidden').first().show(); $('select', row).change(); row.find('.ControlGrid__Index').attr('checked','checked'); var hidden = $('#{1} tbody tr:hidden').length; if(hidden == 0){{ $('#{1}_AddButton').hide(); }}\" class=\"{2} {3} pull-right\"><i class=\"fa fa-plus-square\"></i></button></td></tr></tfoot>", attributes.Count + 1, clientId, controlForm.FormProvider.GetButtonSizeCssClass(ButtonSize.ExtraSmall), controlForm.FormProvider.GetButtonStyleCssClass(ButtonStyle.Success));
                        }
                    }
                }
            }

            sb.Append("</table></div></div>");

            if (EnabledScroll)
            {
                sb.Append("<script type=\"text/javascript\">");
                sb.Append("$(document).ready(function(){");
                sb.AppendFormat("$('#{0}').parent().css('overflow','auto');", clientId);
                sb.Append("});");
                sb.Append("</script>");
            }

            return sb.ToString();
        }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            EnsureProperties();
            return attributes.SelectMany(controlAttribute => controlAttribute.GetAdditionalResources());
        }

        private void EnsureProperties()
        {
            if (typeof(IControlFormProvider).IsAssignableFrom(PropertyType))
            {
                var value = (IControlFormProvider)Value;
                attributes = new List<ControlFormAttribute>(value.GetAttributes());

                foreach (var attribute in attributes)
                {
                    if (string.IsNullOrEmpty(attribute.PropertyName))
                    {
                        attribute.PropertyName = attribute.Name;
                    }
                }

                return;
            }

            if (attributes == null)
            {
                if (!typeof(IEnumerable).IsAssignableFrom(PropertyType) || !PropertyType.IsGenericType)
                {
                    throw new NotSupportedException("Cannot apply control grid for non enumerable property as grid.");
                }

                var type = PropertyType.GetGenericArguments()[0];
                attributes = new List<ControlFormAttribute>();

                foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var attribute = propertyInfo.GetCustomAttribute<ControlFormAttribute>(false);
                    if (attribute != null)
                    {
                        attribute.Name = propertyInfo.Name;
                        attribute.PropertyName = propertyInfo.Name;
                        attribute.PropertyType = propertyInfo.PropertyType;
                        if (attribute.LabelText == null)
                        {
                            attribute.LabelText = propertyInfo.Name;
                        }
                        attributes.Add(attribute);
                    }
                }

                attributes.Sort((x, y) => x.Order.CompareTo(y.Order));
            }
        }

        private static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}