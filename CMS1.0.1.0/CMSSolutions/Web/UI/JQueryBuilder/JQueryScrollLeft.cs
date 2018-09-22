namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryScrollLeft : JQuery
    {
        private readonly object value;

        public JQueryScrollLeft()
        {
        }

        public JQueryScrollLeft(object value)
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
                return ".scrollLeft()";
            }

            if (value is string)
            {
                return string.Format(".scrollLeft(\"{0}\")", JQueryUtility.EncodeJsString(value.ToString()));
            }

            return string.Format(".scrollLeft({0})", value);
        }
    }
}