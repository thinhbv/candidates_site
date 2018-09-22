namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryProperty : JQuery
    {
        private readonly string propertyName;
        private readonly object value;

        public JQueryProperty(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public JQueryProperty(string propertyName, object value)
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
                return string.Format(".prop(\"{0}\")", JQueryUtility.EncodeJsString(propertyName));
            }

            if (value is string)
            {
                return string.Format(".prop(\"{0}\",\"{1}\")", JQueryUtility.EncodeJsString(propertyName), JQueryUtility.EncodeJsString(value.ToString()));
            }

            return string.Format(".prop(\"{0}\",{1})", JQueryUtility.EncodeJsString(propertyName), value);
        }
    }
}