namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryToggleClass : JQuery
    {
        private readonly string className;
        private readonly bool switchs;

        public JQueryToggleClass(string className, bool switchs = false)
        {
            this.className = className;
            this.switchs = switchs;
        }

        public override string Build()
        {
            return string.Format(".toggleClass(\"{0}\",{1})", JQueryUtility.EncodeJsString(className), switchs);
        }
    }
}