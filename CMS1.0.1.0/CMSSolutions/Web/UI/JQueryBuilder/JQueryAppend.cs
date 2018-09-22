namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryAppend : JQuery
    {
        private readonly object content;

        public JQueryAppend(object content)
        {
            this.content = content;
        }

        public override string Build()
        {
            if (content is string)
            {
                return string.Format(".append(\"{0}\")", JQueryUtility.EncodeJsString(content.ToString()));
            }

            return string.Format(".append({0})", content);
        }
    }
}