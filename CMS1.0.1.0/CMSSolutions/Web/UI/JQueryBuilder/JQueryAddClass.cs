namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryAddClass : JQuery
    {
        private readonly string className;

        public JQueryAddClass(string className)
        {
            this.className = className;
        }

        public override string Build()
        {
            return string.Format(".addClass({0})", JQueryUtility.EncodeJsString(className));
        }
    }
}