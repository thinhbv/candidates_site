namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryWrapAll : JQuery
    {
        private readonly object wrappingElement;

        public JQueryWrapAll(object wrappingElement)
        {
            this.wrappingElement = wrappingElement;
        }

        public override string Build()
        {
            if (wrappingElement is string)
            {
                return string.Format(".wrapAll(\"{0}\")", JQueryUtility.EncodeJsString(wrappingElement.ToString()));
            }

            return string.Format(".wrapAll({0})", wrappingElement);
        }
    }
}