namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryWrap : JQuery
    {
        private readonly object wrappingElement;

        public JQueryWrap(object wrappingElement)
        {
            this.wrappingElement = wrappingElement;
        }

        public override string Build()
        {
            if (wrappingElement is string)
            {
                return string.Format(".wrap(\"{0}\")", JQueryUtility.EncodeJsString(wrappingElement.ToString()));
            }

            return string.Format(".wrap({0})", wrappingElement);
        }
    }
}