namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryAfter : JQuery
    {
        private readonly object content;

        public JQueryAfter(object content)
        {
            this.content = content;
        }

        public override string Build()
        {
            if (content is string)
            {
                return string.Format(".after(\"{0}\")", JQueryUtility.EncodeJsString(content.ToString()));
            }

            return string.Format(".after({0})", content);
        }
    }
}