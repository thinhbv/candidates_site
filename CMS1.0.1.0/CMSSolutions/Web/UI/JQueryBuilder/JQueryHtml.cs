namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryHtml : JQuery
    {
        private readonly object htmlString;

        public JQueryHtml()
        {
        }

        public JQueryHtml(object htmlString)
        {
            this.htmlString = htmlString;
        }

        protected override bool ReturnJQuery
        {
            get
            {
                if (htmlString == null)
                {
                    return false;
                }
                return base.ReturnJQuery;
            }
        }

        public override string Build()
        {
            if (htmlString == null)
            {
                return ".html()";
            }

            if (htmlString is string)
            {
                return string.Format(".html(\"{0}\")", JQueryUtility.EncodeJsString(htmlString.ToString()));
            }

            return string.Format(".html({0})", htmlString);
        }
    }
}