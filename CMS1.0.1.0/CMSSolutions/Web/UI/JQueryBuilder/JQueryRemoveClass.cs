namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryRemoveClass : JQuery
    {
        private readonly object className;

        public JQueryRemoveClass(object className)
        {
            this.className = className;
        }

        public override string Build()
        {
            if (className is string)
            {
                return string.Format(".removeClass({0})", JQueryUtility.EncodeJsString(className.ToString()));
            }

            return string.Format(".removeClass({0})", className);
        }
    }
}