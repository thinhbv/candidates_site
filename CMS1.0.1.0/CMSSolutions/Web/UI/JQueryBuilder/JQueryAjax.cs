using System;
using System.Collections.Generic;
using System.Text;

namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryAjax : JQuery
    {
        #region JQueryAjaxDataType enum

        public enum JQueryAjaxDataType
        {
            Xml,
            Json,
            Script,
            Html,
        }

        #endregion JQueryAjaxDataType enum

        private readonly object url;
        private JQuery data;
        private JQueryAjaxDataType? dataType;
        private JQuery[] errorHandlers;
        private JQuery[] successHandlers;
        private JQuery[] completeHandlers;
        private string requestType = "GET";

        public JQueryAjax(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                // Todo: need restore this line
                //throw new ArgumentNullException("url", "The url cannot be null or empty.");
            }
            this.url = url;
        }

        public JQueryAjax(JQuery selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException("selector", "The url cannot be null or empty.");
            }
            url = selector;
        }

        public JQueryAjax Data(JQuery obj)
        {
            data = obj;
            return this;
        }

        public JQueryAjax DataType(JQueryAjaxDataType type)
        {
            dataType = type;
            return this;
        }

        public JQueryAjax OnError(params JQuery[] handlers)
        {
            errorHandlers = handlers;
            return this;
        }

        public JQueryAjax OnSuccess(params JQuery[] handlers)
        {
            successHandlers = handlers;
            return this;
        }

        public JQueryAjax OnComplete(params JQuery[] handlers)
        {
            completeHandlers = handlers;
            return this;
        }

        public JQueryAjax RequestType(string type)
        {
            requestType = type;
            return this;
        }

        public override string Build()
        {
            var config = new List<string>();

            if (url is string)
            {
                config.Add(string.Format("url: \"{0}\"", url));
            }
            else
            {
                config.Add(string.Format("url: {0}", url));
            }

            if (data != null)
            {
                config.Add(string.Format("data: {0}", data));
            }

            if (dataType.HasValue)
            {
                config.Add(string.Format("dataType: \"{0}\"", dataType.Value.ToString().ToLowerInvariant()));
            }

            if (errorHandlers != null && errorHandlers.Length > 0)
            {
                config.Add(string.Format("error: function(jqXHR, textStatus, errorThrown){{ {0} }}", new JQueryHandlers(errorHandlers)));
            }

            if (successHandlers != null && successHandlers.Length > 0)
            {
                config.Add(string.Format("success: function(data, textStatus, jqXHR){{ {0} }}", new JQueryHandlers(successHandlers)));
            }

            if (completeHandlers != null && completeHandlers.Length > 0)
            {
                config.Add(string.Format("complete: function(jqXHR, textStatus){{ {0} }}", new JQueryHandlers(completeHandlers)));
            }

            config.Add(string.Format("type: \"{0}\"", requestType));

            var sb = new StringBuilder();

            sb.Append("jQuery.ajax({");
            sb.Append(string.Join(", ", config));
            sb.Append("});");

            return sb.ToString();
        }
    }
}