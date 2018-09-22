using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Web.UI.JQueryBuilder;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlGridAjaxData<TModel> : List<TModel>
    {
        public ControlGridAjaxData()
        {
            UserData = new Dictionary<string, object>();
            Callbacks = new List<string>();
        }

        public ControlGridAjaxData(IEnumerable<TModel> items)
            : this()
        {
            AddRange(items);
        }

        public ControlGridAjaxData(IEnumerable<TModel> items, int totalRecords)
            : this()
        {
            AddRange(items);
            TotalRecords = totalRecords;
        }

        public int TotalRecords { get; private set; }

        public IDictionary<string, object> UserData { get; set; }

        public IList<string> Callbacks { get; private set; }

        #region Methods

        public void NotifyInfo(string message)
        {
            Callbacks.Add(string.Format("$('body').trigger({{ type: 'SystemMessageEvent', SystemMessage: 'NOTIFY_INFO', Data: {{ Message: {0} }} }});", JQueryUtility.EncodeJsString(message)));
        }

        public void NotifyWarning(string message)
        {
            Callbacks.Add(string.Format("$('body').trigger({{ type: 'SystemMessageEvent', SystemMessage: 'NOTIFY_WARNING', Data: {{ Message: {0} }} }});", JQueryUtility.EncodeJsString(message)));
        }

        public void NotifyError(string message)
        {
            Callbacks.Add(string.Format("$('body').trigger({{ type: 'SystemMessageEvent', SystemMessage: 'NOTIFY_ERROR', Data: {{ Message: {0} }} }});", JQueryUtility.EncodeJsString(message)));
        }

        #endregion

        public static implicit operator ControlGridAjaxData<object>(ControlGridAjaxData<TModel> model)
        {
            var result = new ControlGridAjaxData<object>();
            result.AddRange(model.Cast<object>());
            result.TotalRecords = model.TotalRecords;
            result.UserData = model.UserData;
            return result;
        }
    }
}