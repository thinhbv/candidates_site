namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryScrollTop : JQuery
    {
        private readonly object value;

        public JQueryScrollTop()
        {
        }

        public JQueryScrollTop(object value)
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
                return ".scrollTop()";
            }

            if (value is string)
            {
                return string.Format(".scrollTop(\"{0}\")", JQueryUtility.EncodeJsString(value.ToString()));
            }

            return string.Format(".scrollTop({0})", value);
        }
    }
}