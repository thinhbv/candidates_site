using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;
using CMSSolutions.Extensions;

namespace CMSSolutions.Web.UI.ControlForms
{
    public abstract class ControlSubGridForm
    {
        private readonly IList<ControlGridFormColumn> columns;

        protected ControlSubGridForm()
        {
            columns = new List<ControlGridFormColumn>();
        }

        public IList<ControlGridFormColumn> Columns { get { return columns; } }

        public abstract ControlGridAjaxData<object> GetSubGridData(string parentId);

        public abstract object GetModelId(object item);

        public abstract IList<IControlGridFormRowAction> GetRowActions();

        public string ActionsColumnText { get; set; }

        public int ActionsColumnWidth { get; set; }

        public int? Width { get; set; }

        public JObject AjaxOptions { get; set; }
    }

    public class ControlSubGridForm<TModel, TModelKey> : ControlSubGridForm
    {
        private Func<TModelKey, ControlGridAjaxData<TModel>> fetchDataSource;
        private Func<TModel, object> getModelId;
        private readonly IList<ControlGridFormRowAction<TModel>> rowActions;

        public ControlSubGridForm()
        {
            getModelId = model => model.GetHashCode();
            rowActions = new List<ControlGridFormRowAction<TModel>>();
            ActionsColumnWidth = 120;
        }

        public ControlGridFormColumn<TModel> AddColumn<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            var name = Utils.GetFullPropertyName(expression);
            var column = new ControlGridFormColumn<TModel> { PropertyName = name, HeaderText = name };
            column.SetValueGetter(expression.Compile());
            Columns.Add(column);
            return column;
        }

        public ControlSubGridForm<TModel, TModelKey> HasFetchDataSource(Func<TModelKey, ControlGridAjaxData<TModel>> func)
        {
            fetchDataSource = func;
            return this;
        }

        public ControlSubGridForm<TModel, TModelKey> HasGetModelId(Func<TModel, object> func)
        {
            getModelId = func;
            return this;
        }

        public ControlSubGridForm<TModel, TModelKey> HasWidth(int width)
        {
            Width = width;
            return this;
        }

        public ControlGridFormRowAction<TModel> AddRowAction(bool isSubmitButton = false, bool isAjaxSupport = true)
        {
            var action = new ControlGridFormRowAction<TModel>(isSubmitButton, isAjaxSupport);
            rowActions.Add(action);
            return action;
        }

        public override ControlGridAjaxData<object> GetSubGridData(string parentId)
        {
            TModelKey key;
            var typeOfKey = typeof(TModelKey);
            if (typeOfKey == typeof(Guid))
            {
                key = (TModelKey)(dynamic)(new Guid(parentId));
            }
            else
            {
                key = parentId.ConvertTo<TModelKey>();
            }

            return fetchDataSource(key);
        }

        public override object GetModelId(object item)
        {
            return getModelId((TModel)item);
        }

        public override IList<IControlGridFormRowAction> GetRowActions()
        {
            return rowActions.Select(x => (IControlGridFormRowAction)x).ToList();
        }
    }
}