using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace CMSSolutions.Web.Mvc.Html
{
    public class MvcAjaxForm : MvcForm
    {
        private readonly HtmlHelper htmlHelper;
        private readonly string formAction;
        private string confirmMessage;
        private string httpMethod;
        private string onFailure;
        private string onSuccess;

        public MvcAjaxForm(HtmlHelper htmlHelper, string formAction, IDictionary<string, object> htmlAttributes)
            : base(htmlHelper.ViewContext)
        {
            this.htmlHelper = htmlHelper;
            this.formAction = formAction;
            httpMethod = "POST";

            BuildMvcAjaxForm(htmlAttributes);
        }

        public MvcAjaxForm HttpMethod(string method)
        {
            httpMethod = method;
            return this;
        }

        public MvcAjaxForm ConfirmMessage(string message)
        {
            confirmMessage = message;
            return this;
        }

        public MvcAjaxForm OnSuccess(string clientFunction)
        {
            onSuccess = clientFunction;
            return this;
        }

        public MvcAjaxForm OnFailure(string clientFunction)
        {
            onFailure = clientFunction;
            return this;
        }

        private void BuildMvcAjaxForm(IDictionary<string, object> htmlAttributes)
        {
            var builder = new TagBuilder("form");
            builder.MergeAttribute("action", formAction);
            builder.MergeAttribute("method", httpMethod);
            builder.MergeAttribute("data-ajax", "true");
            builder.MergeAttributes(htmlAttributes, true);

            if (!string.IsNullOrEmpty(confirmMessage))
            {
                builder.MergeAttribute("data-ajax-confirm", confirmMessage);
            }

            if (!string.IsNullOrEmpty(onSuccess))
            {
                builder.MergeAttribute("data-ajax-success", onSuccess);
            }

            if (!string.IsNullOrEmpty(onFailure))
            {
                builder.MergeAttribute("data-ajax-failure", onFailure);
            }

            htmlHelper.ViewContext.Writer.Write(builder.ToString(TagRenderMode.StartTag));
        }
    }
}