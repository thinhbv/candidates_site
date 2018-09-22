namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryAttr : JQuery
    {
        private readonly string attributeName;
        private readonly object value;

        public JQueryAttr(string attributeName)
        {
            this.attributeName = attributeName;
        }

        public JQueryAttr(string attributeName, object value)
        {
            this.attributeName = attributeName;
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
                return string.Format(".attr(\"{0}\")", JQueryUtility.EncodeJsString(attributeName));
            }

            if (value is string)
            {
                return string.Format(".attr(\"{0}\",\"{1}\")", JQueryUtility.EncodeJsString(attributeName), JQueryUtility.EncodeJsString(value.ToString()));
            }

            return string.Format(".attr(\"{0}\",{1})", JQueryUtility.EncodeJsString(attributeName), value);
        }
    }
}