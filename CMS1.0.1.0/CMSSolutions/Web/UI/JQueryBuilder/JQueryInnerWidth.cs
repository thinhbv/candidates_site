namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryInnerWidth : JQuery
    {
        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            return ".innerWidth()";
        }
    }
}