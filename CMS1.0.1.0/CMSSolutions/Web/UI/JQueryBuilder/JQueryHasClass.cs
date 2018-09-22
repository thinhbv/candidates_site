namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryHasClass : JQuery
    {
        private readonly string className;

        public JQueryHasClass(string className)
        {
            this.className = className;
        }

        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            return string.Format(".hasClass(\"{0}\")", JQueryUtility.EncodeJsString(className));
        }
    }
}