namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryWidth : JQuery
    {
        private readonly object value;

        public JQueryWidth()
        {
        }

        public JQueryWidth(object value)
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
                return ".width()";
            }

            if (value is string)
            {
                return string.Format(".width(\"{0}\")", JQueryUtility.EncodeJsString(value.ToString()));
            }

            return string.Format(".width({0})", value);
        }
    }
}