using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class TableGridControlComponent<TModel> : IControlComponent
    {
        private readonly IEnumerable<TModel> items;
        private readonly IList<ControlGridFormColumn> columns;
        private readonly IList<ControlFormAction> actions;
        private readonly IList<ControlGridFormRowAction<TModel>> rowActions;

        public TableGridControlComponent(IEnumerable<TModel> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            this.items = items;
            columns = new List<ControlGridFormColumn>();
            actions = new List<ControlFormAction>();
            rowActions = new List<ControlGridFormRowAction<TModel>>();
            NumberOfPageLinks = 5;
            IsAjaxSupported = true;
        }

        public string Title { get; set; }

        public string CssClass { get; set; }

        public int TotalItemsCount { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int NumberOfPageLinks { get; set; }

        public string ViewActionName { get; set; }

        public string UpdateActionName { get; set; }

        public bool IsAjaxSupported { get; set; }

        public string ActionsColumnWidth { get; set; }

        public ControlGridFormColumn AddColumn(Expression<Func<TModel, dynamic>> expression)
        {
            var column = new ControlGridFormColumn(expression);
            columns.Add(column);
            return column;
        }

        public ControlFormAction AddAction(bool isSubmitButton = false, bool isAjaxSupport = true)
        {
            var action = new ControlFormAction(isSubmitButton, isAjaxSupport);
            actions.Add(action);
            return action;
        }

        public ControlGridFormRowAction<TModel> AddRowAction(bool isSubmitButton = false, bool isAjaxSupport = true)
        {
            var action = new ControlGridFormRowAction<TModel>(isSubmitButton, isAjaxSupport);
            rowActions.Add(action);
            return action;
        }

        public void Build(StringBuilder stringBuilder)
        {
            throw new NotImplementedException();
        }

        public class ControlGridFormColumn
        {
            private readonly Expression<Func<TModel, dynamic>> expression;

            internal Expression<Func<TModel, dynamic>> Expression
            {
                get { return expression; }
            }

            internal string HeaderText { get; set; }

            internal string DisplayFormat { get; set; }

            internal bool IsCheckbox { get; set; }

            internal string ControlName { get; set; }

            internal string Width { get; set; }

            internal Func<TModel, MvcHtmlString> HtmlBuilder { get; private set; }

            internal ControlGridFormColumn(Expression<Func<TModel, dynamic>> expression)
            {
                this.expression = expression;
            }

            public ControlGridFormColumn HasDisplayFormat(string value)
            {
                DisplayFormat = value;
                return this;
            }

            public ControlGridFormColumn HasHeaderText(string value)
            {
                HeaderText = value;
                return this;
            }

            public ControlGridFormColumn RenderAsCheckbox()
            {
                IsCheckbox = true;
                return this;
            }

            public ControlGridFormColumn HasWidth(string value)
            {
                Width = value;
                return this;
            }

            public ControlGridFormColumn HtmlRender(Func<TModel, MvcHtmlString> builder)
            {
                HtmlBuilder = builder;
                return this;
            }
        }
    }
}