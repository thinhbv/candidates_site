using System;
using System.Globalization;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlNumericAttribute : ControlFormAttribute
    {
        /// <summary>
        /// The minimum value.
        /// </summary>
        public string MinimumValue { get; set; }

        /// <summary>
        /// The maximum value.
        /// </summary>
        public string MaximumValue { get; set; }

        /// <summary>
        /// The maximum text length.
        /// </summary>
        public int MaxLength { get; set; }

        public override string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return controlForm.FormProvider.GenerateControlUI(controlForm, this, workContext, htmlHelper);
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            var attributes = new RouteValueDictionary();

            var cssClass = (controlForm.FormProvider.ControlCssClass + " " + CssClass).Trim();
            if (!string.IsNullOrEmpty(cssClass))
            {
                attributes.Add("class", cssClass);
            }

            if (!string.IsNullOrEmpty(DataBind))
            {
                attributes.Add("data-bind", DataBind);
            }

            if (ReadOnly || controlForm.ReadOnly)
            {
                attributes.Add("readonly", "readonly");
            }
            else
            {
                attributes.Add("data-val", "true");
                attributes.Add("data-val-number", Constants.Messages.Validation.Number);

                if (Required)
                {
                    attributes.Add("data-val-required", Constants.Messages.Validation.Required);
                }

                if (MaxLength > 0)
                {
                    attributes.Add("maxlength", MaxLength);
                }

                var typeCode = Type.GetTypeCode(PropertyType);
                if (typeCode == TypeCode.Object)
                {
                    if (PropertyType.Name == "Nullable`1")
                    {
                        typeCode = Type.GetTypeCode(PropertyType.GetGenericArguments()[0]);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }

                string minimumValue = null;
                string maximumValue = null;

                switch (typeCode)
                {
                    case TypeCode.SByte:
                        minimumValue = "-128";
                        maximumValue = "127";
                        break;

                    case TypeCode.Byte:
                        minimumValue = "0";
                        maximumValue = "255";
                        break;

                    case TypeCode.Int16:
                        minimumValue = "-32768";
                        maximumValue = "32767";
                        break;

                    case TypeCode.UInt16:
                        minimumValue = "0";
                        maximumValue = "65535";
                        break;

                    case TypeCode.Int32:
                        minimumValue = "-2147483648";
                        maximumValue = "2147483647";
                        break;

                    case TypeCode.UInt32:
                        minimumValue = "0";
                        maximumValue = "4294967295";
                        break;

                    case TypeCode.Int64:
                        minimumValue = "-9223372036854775808";
                        maximumValue = "9223372036854775807";
                        break;

                    case TypeCode.UInt64:
                        minimumValue = "0";
                        maximumValue = "18446744073709551615";
                        break;

                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        break;
                }

                if (!string.IsNullOrEmpty(MinimumValue))
                {
                    minimumValue = MinimumValue;

                    if (minimumValue == "{YearNow}")
                    {
                        minimumValue = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
                    }
                }

                if (!string.IsNullOrEmpty(MaximumValue))
                {
                    maximumValue = MaximumValue;

                    if (maximumValue == "{YearNow}")
                    {
                        maximumValue = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
                    }
                }

                if (!string.IsNullOrEmpty(minimumValue) && !string.IsNullOrEmpty(maximumValue))
                {
                    attributes.Add("data-val-range-min", minimumValue);
                    attributes.Add("data-val-range-max", maximumValue);
                    attributes.Add("data-val-range", string.Format(Constants.Messages.Validation.Range, minimumValue, maximumValue));    
                }
                else if(!string.IsNullOrEmpty(minimumValue))
                {
                    attributes.Add("data-val-range-min", minimumValue);
                    attributes.Add("data-val-range", string.Format(Constants.Messages.Validation.RangeMin, minimumValue));    
                }
                else if(!string.IsNullOrEmpty(maximumValue))
                {
                    attributes.Add("data-val-range-max", minimumValue);
                    attributes.Add("data-val-range", string.Format(Constants.Messages.Validation.RangeMax, maximumValue));    
                }
            }

            var valMsg = new MvcHtmlString(string.Format("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", Name));

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(PrependText))
            {
                sb.Append(string.Format("<span class=\"help-inline\">{0}</span>", PrependText));
            }

            sb.Append(htmlHelper.TextBox(Name, Value, attributes));

            if (!string.IsNullOrEmpty(AppendText))
            {
                sb.AppendFormat("<span class=\"help-inline\">{0}</span>", AppendText);
            }

            if (!string.IsNullOrEmpty(HelpText))
            {
                sb.AppendFormat("<span class=\"help-block\">{0}</span>", HelpText);
            }

            sb.Append(valMsg);

            return sb.ToString();
        }
    }
}