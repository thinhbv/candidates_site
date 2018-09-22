namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryOuterWidth : JQuery
    {
        private readonly object includeMargin;

        public JQueryOuterWidth()
        {
        }

        public JQueryOuterWidth(object includeMargin)
        {
            this.includeMargin = includeMargin;
        }

        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            if (includeMargin == null)
            {
                return ".outerWidth()";
            }

            if (includeMargin is string)
            {
                return string.Format(".outerWidth(\"{0}\")", JQueryUtility.EncodeJsString(includeMargin.ToString()));
            }

            return string.Format(".outerWidth({0})", includeMargin);
        }
    }
}