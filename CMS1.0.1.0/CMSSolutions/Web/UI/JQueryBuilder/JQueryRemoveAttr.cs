namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryRemoveAttr : JQuery
    {
        private readonly string attributeName;

        public JQueryRemoveAttr(string attributeName)
        {
            this.attributeName = attributeName;
        }

        public override string Build()
        {
            return string.Format(".removeAttr(\"{0}\")", JQueryUtility.EncodeJsString(attributeName));
        }
    }
}