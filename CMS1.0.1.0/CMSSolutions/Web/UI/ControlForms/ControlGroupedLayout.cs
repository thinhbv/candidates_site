using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlGroupedLayout<TModel>
    {
        private readonly IList<string> properties;
        private readonly string title;

        public string Title { get { return title; } }

        public bool EnableScrollbar { get; set; }

        public int Column { get; set; }

        public bool EnableGrid { get; set; }

        public string CssClass { get; set; }

        public IList<string> Properties { get { return properties; } }

        internal ControlGroupedLayout(string title)
        {
            properties = new List<string>();
            this.title = title;
            Column = 1;
        }

        public ControlGroupedLayout<TModel> Add(string property)
        {
            properties.Add(property);
            return this;
        }

        public ControlGroupedLayout<TModel> Add<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            properties.Add(ExpressionHelper.GetExpressionText(expression));
            return this;
        }
    }
}