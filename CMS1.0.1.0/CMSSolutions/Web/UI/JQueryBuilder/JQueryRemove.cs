namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryRemove : JQuery
    {
        private readonly object selector;

        public JQueryRemove()
        {
        }

        public JQueryRemove(object selector)
        {
            this.selector = selector;
        }

        public override string Build()
        {
            if (selector == null)
            {
                return ".remove()";
            }

            if (selector is string)
            {
                return string.Format(".remove(\"{0}\")", JQueryUtility.EncodeJsString(selector.ToString()));
            }

            return string.Format(".remove({0})", selector);
        }
    }
}