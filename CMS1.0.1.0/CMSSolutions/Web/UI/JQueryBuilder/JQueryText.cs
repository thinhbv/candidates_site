namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryText : JQuery
    {
        private readonly object textString;

        public JQueryText()
        {
        }

        public JQueryText(object textString)
        {
            this.textString = textString;
        }

        protected override bool ReturnJQuery
        {
            get
            {
                if (textString == null)
                {
                    return false;
                }
                return base.ReturnJQuery;
            }
        }

        public override string Build()
        {
            if (textString == null)
            {
                return ".text()";
            }

            if (textString is string)
            {
                return string.Format(".text(\"{0}\")", JQueryUtility.EncodeJsString(textString.ToString()));
            }

            return string.Format(".text({0})", textString);
        }
    }
}