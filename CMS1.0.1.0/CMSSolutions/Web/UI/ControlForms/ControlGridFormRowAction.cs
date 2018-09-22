using System;
using System.Collections.Generic;
using System.Linq;

namespace CMSSolutions.Web.UI.ControlForms
{
    public interface IControlGridFormRowAction
    {
        string Name { get; }

        string Text { get; }

        bool IsSubmitButton { get; }

        bool IsShowModalDialog { get; }

        int ModalDialogWidth { get; }

        int ModalDialogHeight { get; }

        ButtonSize ButtonSize { get; }

        ButtonStyle ButtonStyle { get; }

        string CssClass { get; }

        string ConfirmMessage { get; }

        string ClientClickCode { get; set; }

        bool IsVisible(object item);

        bool IsEnable(object item);

        object GetValue(object item);

        string GetUrl(object item);

        IDictionary<string, object> GetAttributes(object obj);
    }

    public class ControlGridFormRowAction<TModel> : ControlFormActionBase<ControlGridFormRowAction<TModel>>, IControlGridFormRowAction
    {
        private readonly IDictionary<string, Func<TModel, object>> attributes;

        public ControlGridFormRowAction(bool isSubmitButton, bool isAjaxSupport)
            : base(isSubmitButton, isAjaxSupport)
        {
            attributes = new Dictionary<string, Func<TModel, object>>();
        }

        public Func<TModel, object> ValueSelector { get; set; }

        public Func<TModel, string> UrlBuilder { get; set; }

        public Func<TModel, bool> VisibleWhenFunc { get; set; }

        public Func<TModel, bool> EnableWhenFunc { get; set; }

        public ControlGridFormRowAction<TModel> HasValue(Func<TModel, object> valueSelector)
        {
            ValueSelector = valueSelector;
            return this;
        }

        public ControlGridFormRowAction<TModel> HasUrl(Func<TModel, string> urlBuilder)
        {
            IsSubmitButton = false;
            UrlBuilder = urlBuilder;
            return this;
        }

        public ControlGridFormRowAction<TModel> VisibleWhen(Func<TModel, bool> func)
        {
            VisibleWhenFunc = func;
            return this;
        }

        public ControlGridFormRowAction<TModel> EnableWhen(Func<TModel, bool> func)
        {
            EnableWhenFunc = func;
            return this;
        }

        public ControlGridFormRowAction<TModel> HasAttribute(string name, Func<TModel, object> func)
        {
            attributes.Add(name, func);
            return this;
        }

        public bool IsVisible(object item)
        {
            return VisibleWhenFunc == null || VisibleWhenFunc((TModel)item);
        }

        public bool IsEnable(object item)
        {
            return EnableWhenFunc == null || EnableWhenFunc((TModel)item);
        }

        public object GetValue(object item)
        {
            return ValueSelector != null ? ValueSelector((TModel)item) : null;
        }

        public string GetUrl(object item)
        {
            return UrlBuilder != null ? UrlBuilder((TModel)item) : null;
        }

        public IDictionary<string, object> GetAttributes(object obj)
        {
            var model = (TModel)obj;
            var dic = attributes.ToDictionary(k => k.Key, v => v.Value(model));
            var keys = (from pair in dic where pair.Value == null select pair.Key).ToList();
            foreach (var key in keys)
            {
                dic.Remove(key);
            }
            return dic;
        }
    }
}