namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryRemoveProperty : JQuery
    {
        private readonly string propertyName;

        public JQueryRemoveProperty(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public override string Build()
        {
            return string.Format(".removeProp(\"{0}\")", JQueryUtility.EncodeJsString(propertyName));
        }
    }
}