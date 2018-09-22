using System;

namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryAlert : JQuery
    {
        private readonly object message;

        public JQueryAlert(string message)
        {
            this.message = message;
        }

        public JQueryAlert(object obj)
        {
            message = obj;
        }

        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            if (message == null)
            {
                throw new ArgumentException("Cannot alert a null message.");
            }

            var str = message as string;
            if (str != null)
            {
                return string.Format("alert('{0}');", str.Replace("'", "\'"));
            }

            return string.Format("alert({0});", message);
        }
    }
}