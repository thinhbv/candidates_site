namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryBefore : JQuery
    {
        private readonly object content;

        public JQueryBefore()
        {
        }

        public JQueryBefore(object content)
        {
            this.content = content;
        }

        protected override bool ReturnJQuery
        {
            get
            {
                if (content == null)
                {
                    return false;
                }
                return base.ReturnJQuery;
            }
        }

        public override string Build()
        {
            if (content == null)
            {
                return ".before()";
            }

            if (content is string)
            {
                return string.Format(".before(\"{0}\")", JQueryUtility.EncodeJsString(content.ToString()));
            }

            return string.Format(".before({0})", content);
        }
    }
}