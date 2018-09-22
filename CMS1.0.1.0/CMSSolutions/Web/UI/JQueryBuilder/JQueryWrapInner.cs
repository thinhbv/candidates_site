namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryWrapInner : JQuery
    {
        private readonly object wrappingElement;

        public JQueryWrapInner(object wrappingElement)
        {
            this.wrappingElement = wrappingElement;
        }

        public override string Build()
        {
            if (wrappingElement is string)
            {
                return string.Format(".wrapInner(\"{0}\")", JQueryUtility.EncodeJsString(wrappingElement.ToString()));
            }

            return string.Format(".wrapInner({0})", wrappingElement);
        }
    }
}