namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryOuterHeight : JQuery
    {
        private readonly object includeMargin;

        public JQueryOuterHeight()
        {
        }

        public JQueryOuterHeight(object includeMargin)
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
                return ".outerHeight()";
            }

            if (includeMargin is string)
            {
                return string.Format(".outerHeight(\"{0}\")", JQueryUtility.EncodeJsString(includeMargin.ToString()));
            }

            return string.Format(".outerHeight({0})", includeMargin);
        }
    }
}