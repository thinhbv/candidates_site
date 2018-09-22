using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlDatePickerAttribute : ControlFormAttribute
    {
        private int? yearRangeMinOffset;
        private int? yearRangeMaxOffset;

        public ControlDatePickerAttribute()
        {
            ShowOn = "both";
        }

        public ControlDatePickerAttribute(int yearRangeMinOffset, int yearRangeMaxOffset) : this()
        {
            this.yearRangeMinOffset = yearRangeMinOffset;
            this.yearRangeMaxOffset = yearRangeMaxOffset;
        }
        //TODO: rename this.. it's very bad
        public string ToChildrenDate { get; set; }

        /// <summary>
        /// The format for parsed and displayed date.
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// The range of years displayed in the year drop-down of date picker control.
        /// </summary>
        public string YearRangeFormat { get; set; }

        /// <summary>
        /// The minimum value.
        /// </summary>
        public string MinimumValue { get; set; }

        /// <summary>
        /// The maximum value.
        /// </summary>
        public string MaximumValue { get; set; }

        /// <summary>
        /// The property name of start date range
        /// </summary>
        public string StartDateRange { get; set; }

        /// <summary>
        /// The property name of end date range
        /// </summary>
        public string EndDateRange { get; set; }

        /// <summary>
        /// Sort Required
        /// </summary>
        /// <returns></returns>
        public bool EnableSortRequired { get; set; }

        public string ShowOn { get; set; }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            yield return ResourceType.JQueryUI;
        }

        public override string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return controlForm.FormProvider.GenerateControlUI(controlForm, this, workContext, htmlHelper);
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            var id = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(Name);

            var dateFormat = DateFormat;
            if (string.IsNullOrEmpty(dateFormat))
            {
                dateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            }
            string formatedValue = null;

            var rawValue = Value;
            if (rawValue is DateTime)
            {
                var dtValue = (DateTime)rawValue;
                formatedValue = dtValue != DateTime.MinValue ? dtValue.ToString(dateFormat) : string.Empty;
            }
            else if (rawValue is string)
            {
                formatedValue = rawValue as string;
            }

            var attributes = new RouteValueDictionary { { "autocomplete", "off" } };

            var cssClass = (controlForm.FormProvider.ControlCssClass + " " + CssClass + " datepicker").Trim();
            if (!string.IsNullOrEmpty(cssClass))
            {
                attributes.Add("class", cssClass);
            }

            if (controlForm.ReadOnly || ReadOnly)
            {
                attributes.Add("readonly", "readonly");
                return htmlHelper.TextBox(Name, formatedValue, attributes).ToHtmlString();
            }

            attributes.Add("data-val", "true");
            attributes.Add("data-val-date", Constants.Messages.Validation.Date);

            if (Required)
            {
                attributes.Add("data-val-required",
                               EnableSortRequired ? "*" : Constants.Messages.Validation.Required);
            }

            attributes.Add("data-jqui-type", "datepicker");
            attributes.Add("data-jqui-dpicker-showon", ShowOn);
            attributes.Add("data-jqui-dpicker-buttonimageonly", "true");
            attributes.Add("data-jqui-dpicker-buttonimage", "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAdlJREFUeNqUU71OAkEQnpWNJ0QKAyJ4oIZTNBbG5AobLKC3Mr6AtaHwASgoLX0CazW+gbGAxsLEwqgRMAZQIoZgYc4/7taZ5e7EePHnSyY3Oz/ffnO7y+qMAQOA6ugobC8vL6E7h3aZKxaPpx8egFDxyGmYE7hgNkEB/fmyEFPq1ZV+m0qdzDB2A33wyJ0jQZ6/9fKLyUJhxTg6gna7DWo2q6vptN5PYJRKn7lMRr/O5wcpzl96ecV6egJleBj4xgYoqgrdx8f+/i85qsU+RRIYgiYBv2kYoG5tQejsDCzThDYWBnd2XIJwJCKNQLXY5/+qAIONXA78gYAMPON6KBwGL/hwU0eB/K1omwJh/dEI1EO9XNikJrIeHB4C5sCyLPdr4jj07Y+tr67SEfqocYAIaNHFBCGZTEr7yada6qFe7oxFQSrQolEZoN1m43Hp064Lmub69mZSgTMCf7cVVJpNWUS4qNXkCITTclmSEt57CrjnCGMjIxALhaQfx2ObjMWkryUSMD0x8X0ER0HXZr/vdFwFjVbLVVCt110FVOso4M/4FobwTH2cQ9S+KL+BagVjCt0hfifE+CRjwd39ffgPUFeQeukl6xHG1vBaJf5D8ApQbwmx9yHAAOL/JyG8uQSRAAAAAElFTkSuQmCC"); 
            attributes.Add("data-jqui-dpicker-dateformat", ConvertDateFormat(dateFormat));

            if (!string.IsNullOrEmpty(MinimumValue))
            {
                attributes.Add("data-jqui-dpicker-mindate", MinimumValue);
            }

            if (!string.IsNullOrEmpty(MaximumValue))
            {
                attributes.Add("data-jqui-dpicker-maxdate", MaximumValue);
            }

            if (!string.IsNullOrEmpty(YearRangeFormat))
            {
                attributes.Add("data-jqui-dpicker-changemonth", "true");
                attributes.Add("data-jqui-dpicker-changeyear", "true");
                attributes.Add("data-jqui-dpicker-yearrange", YearRangeFormat);
            }
            else if (yearRangeMinOffset.HasValue && yearRangeMaxOffset.HasValue)
            {
                attributes.Add("data-jqui-dpicker-changemonth", "true");
                attributes.Add("data-jqui-dpicker-changeyear", "true");

                var dt = DateTime.Now.Year;
                attributes.Add("data-jqui-dpicker-yearrange", string.Format("{0}:{1}", dt + yearRangeMinOffset.Value, dt + yearRangeMaxOffset.Value));
            }

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(StartDateRange))
            {
                attributes.Add("data-jqui-dpicker-onclose", id + "_OnClose");

                sb.AppendFormat("function {0}_OnClose(selectedDate){{ $('#{1}').datepicker('option', 'maxDate', selectedDate);  }}", id, id.Replace(Name, StartDateRange));
            }

            if (!string.IsNullOrEmpty(EndDateRange))
            {
                if (sb.Length > 0)
                {
                    throw new NotSupportedException("Cannot set value for both StartDateRange and EndDateRange.");
                }

                attributes.Add("data-jqui-dpicker-onclose", id + "_OnClose");
                sb.AppendFormat("function {0}_OnClose(selectedDate){{ $('#{1}').datepicker('option', 'minDate', selectedDate);  }}", id, id.Replace(Name, EndDateRange));
            }

            if (!string.IsNullOrEmpty(ToChildrenDate))
            {
                attributes.Add("data-jqui-dpicker-onclose", id + "_OnClose");
                var idDateChild = id.Replace(PropertyName ?? Name, ToChildrenDate);
                sb.AppendFormat("function {0}_OnClose(selectedDate){{ $('#{1}').datepicker('option', 'minDate', selectedDate); var currentValue = $('#{1}').val(); if(!currentValue && currentValue.length == 0) $('#{1}').val(selectedDate);  }}", id, idDateChild);
            }

            if (sb.Length > 0)
            {
                sb.Insert(0, "<script type=\"text/javascript\">");
                sb.Append("</script>");
            }

            var sb2 = new StringBuilder();

            if (!string.IsNullOrEmpty(PrependText))
            {
                sb2.Append(string.Format("<span class=\"help-inline\">{0}</span>", PrependText));
            }

            sb2.Append(htmlHelper.TextBox(Name, formatedValue, attributes));

            if (!string.IsNullOrEmpty(AppendText))
            {
                sb2.AppendFormat("<span class=\"help-inline\">{0}</span>", AppendText);
            }

            sb2.AppendFormat("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", Name);

            if (!string.IsNullOrEmpty(HelpText))
            {
                sb2.AppendFormat("<span class=\"help-block\">{0}</span>", HelpText);
            }

            sb2.Append(sb);

            return sb2.ToString();
        }

        private static string ConvertDateFormat(string format)
        {
            /*
             *  .NET    JQueryUI        Output      Comment
             *  --------------------------------------------------------------
             *  d       d               5           day of month(No leading zero)
             *  dd      dd              05          day of month(two digit)
             *  ddd     D               Thu         day short name
             *  dddd    DD              Thursday    day long name
             *  M       m               11          month of year(No leading zero)
             *  MM      mm              11          month of year(two digit)
             *  MMM     M               Nov         month name short
             *  MMMM    MM              November    month name long.
             *  yy      y               09          Year(two digit)
             *  yyyy    yy              2009        Year(four digit)
             */

            var currentFormat = format;

            // Convert the date
            currentFormat = currentFormat.Replace("dddd", "DD");
            currentFormat = currentFormat.Replace("ddd", "D");

            // Convert month
            if (currentFormat.Contains("MMMM"))
            {
                currentFormat = currentFormat.Replace("MMMM", "MM");
            }
            else if (currentFormat.Contains("MMM"))
            {
                currentFormat = currentFormat.Replace("MMM", "M");
            }
            else if (currentFormat.Contains("MM"))
            {
                currentFormat = currentFormat.Replace("MM", "mm");
            }
            else
            {
                currentFormat = currentFormat.Replace("M", "m");
            }

            // Convert year
            currentFormat = currentFormat.Contains("yyyy") ? currentFormat.Replace("yyyy", "yy") : currentFormat.Replace("yy", "y");

            return currentFormat;
        }
    }
}