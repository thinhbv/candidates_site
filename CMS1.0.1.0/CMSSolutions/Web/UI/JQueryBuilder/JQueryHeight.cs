namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryHeight : JQuery
    {
        private readonly object value;

        public JQueryHeight()
        {
        }

        public JQueryHeight(object value)
        {
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
                return ".height()";
            }

            if (value is string)
            {
                return string.Format(".height(\"{0}\")", JQueryUtility.EncodeJsString(value.ToString()));
            }

            return string.Format(".height({0})", value);
        }
    }
}