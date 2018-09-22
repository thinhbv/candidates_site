namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryValue : JQuery
    {
        private readonly object value;

        internal JQueryValue()
        {
        }

        internal JQueryValue(object value)
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
                return ".val()";
            }

            if (value is string)
            {
                return string.Format(".val(\"{0}\")", JQueryUtility.EncodeJsString(value.ToString()));
            }

            return string.Format(".val({0})", value);
        }
    }
}