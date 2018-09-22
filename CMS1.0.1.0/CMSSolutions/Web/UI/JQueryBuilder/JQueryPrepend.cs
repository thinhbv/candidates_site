namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryPrepend : JQuery
    {
        private readonly object content;

        public JQueryPrepend()
        {
        }

        public JQueryPrepend(object content)
        {
            this.content = content;
        }

        public override string Build()
        {
            if (content == null)
            {
                return ".prepend()";
            }

            if (content is string)
            {
                return string.Format(".prepend(\"{0}\")", JQueryUtility.EncodeJsString(content.ToString()));
            }

            return string.Format(".prepend({0})", content);
        }
    }
}