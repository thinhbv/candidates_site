using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlComplexAttribute : ControlFormAttribute
    {
        public ControlComplexAttribute()
        {
            Column = 1;
            EnableGrid = false;
        }

        public override bool HasLabelControl
        {
            get { return false; }
        }

        public int Column { get; set; }

        public bool EnableGrid { get; set; }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            var attributes = new List<ControlFormAttribute>();
            foreach (var propertyInfo in PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var attribute = propertyInfo.GetCustomAttribute<ControlFormAttribute>();
                if (attribute != null)
                {
                    attribute.Name = Name + "." + propertyInfo.Name;
                    attribute.PropertyName = propertyInfo.Name;
                    attribute.PropertyType = propertyInfo.PropertyType;
                    attribute.PropertyInfo = propertyInfo;
                    attributes.Add(attribute);
                }
            }

            if (EnableGrid)
            {
                var groupedLayoutColumns = Column;

                if (groupedLayoutColumns == 0)
                {
                    groupedLayoutColumns = 1;
                }

                var groupedLayoutRows = (int)Math.Ceiling((double)attributes.Count / groupedLayoutColumns);

                controlForm.FormProvider.WriteToOutput("<table style=\"width: 100%;\">");

                var columnWith = 100 / groupedLayoutColumns;

                controlForm.FormProvider.WriteToOutput("<colgroup>");

                for (int i = 0; i < groupedLayoutColumns; i++)
                {
                    controlForm.FormProvider.WriteToOutput(string.Format("<col style=\"width: {0}%\">", columnWith));
                }

                controlForm.FormProvider.WriteToOutput("</colgroup>");

                var index = 0;

                for (var i = 0; i < groupedLayoutRows; i++)
                {
                    controlForm.FormProvider.WriteToOutput("<tr>");

                    for (var j = 0; j < groupedLayoutColumns; j++)
                    {
                        if (index == attributes.Count)
                        {
                            continue;
                        }
                        var attribute = attributes[index];

                        attribute.Value = controlForm.GetPropertyValue(Value, attribute.PropertyName);

                        if (!string.IsNullOrEmpty(attribute.ControlSpan))
                        {
                            continue;
                        }

                        var controlFormAttributes = attributes.Where(x => x.ControlSpan == attribute.PropertyName).ToList();
                        foreach (var controlFormAttribute in controlFormAttributes)
                        {
                            controlFormAttribute.Value = controlForm.GetPropertyValue(Value, controlFormAttribute.PropertyName);
                        }

                        var spanControls = controlFormAttributes.Select(x => x.GenerateControlUI(controlForm, workContext, htmlHelper)).ToList();
                        spanControls.Insert(0, attribute.GenerateControlUI(controlForm, workContext, htmlHelper));

                        if (attribute is ControlHiddenAttribute)
                        {
                            controlForm.FormProvider.WriteToOutput(attribute, spanControls.ToArray());
                            index++;
                            j--;
                            continue;
                        }

                        controlForm.FormProvider.WriteToOutput("<td>");

                        controlForm.FormProvider.WriteToOutput(attribute, spanControls.ToArray());

                        controlForm.FormProvider.WriteToOutput("</td>");
                        index++;
                    }

                    controlForm.FormProvider.WriteToOutput("</tr>");
                }

                controlForm.FormProvider.WriteToOutput("</table>");
            }
            else
            {
                foreach (var attribute in attributes)
                {
                    if (ReadOnly)
                    {
                        attribute.ReadOnly = true;
                    }

                    attribute.Value = controlForm.GetPropertyValue(Value, attribute.PropertyName);

                    if (!string.IsNullOrEmpty(attribute.ControlSpan))
                    {
                        continue;
                    }

                    var controlFormAttributes = attributes.Where(x => x.ControlSpan == attribute.PropertyName).ToList();
                    foreach (var controlFormAttribute in controlFormAttributes)
                    {
                        controlFormAttribute.Value = controlForm.GetPropertyValue(Value, controlFormAttribute.PropertyName);
                    }

                    var spanControls = controlFormAttributes.Select(x => x.GenerateControlUI(controlForm, workContext, htmlHelper)).ToList();

                    spanControls.Insert(0, attribute.GenerateControlUI(controlForm, workContext, htmlHelper));

                    controlForm.FormProvider.WriteToOutput(attribute, spanControls.ToArray());
                }
            }

            return null;
        }
    }
}