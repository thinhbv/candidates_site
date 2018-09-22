namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryCss : JQuery
    {
        private readonly string propertyName;
        private readonly string value;

        public JQueryCss(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public JQueryCss(string propertyName, string value)
        {
            this.propertyName = propertyName;
            this.value = value;
        }

        protected override bool ReturnJQuery
        {
            get
            {
                if (value == null)
                {
                    return false;
                }
                return base.ReturnJQuery;
            }
        }

        public override string Build()
        {
            if (value == null)
            {
                return string.Format(".css(\"{0}\")", JQueryUtility.EncodeJsString(propertyName));
            }

            return string.Format(".css(\"{0}\", \"{1}\")", JQueryUtility.EncodeJsString(propertyName), JQueryUtility.EncodeJsString(value));
        }
    }
}