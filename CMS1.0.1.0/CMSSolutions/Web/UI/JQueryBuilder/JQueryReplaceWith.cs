namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryReplaceWith : JQuery
    {
        private readonly object newContent;

        public JQueryReplaceWith(object newContent)
        {
            this.newContent = newContent;
        }

        public override string Build()
        {
            if (newContent is string)
            {
                return string.Format(".replaceWith(\"{0}\")", JQueryUtility.EncodeJsString(newContent.ToString()));
            }

            return string.Format(".replaceWith({0})", newContent);
        }
    }
}