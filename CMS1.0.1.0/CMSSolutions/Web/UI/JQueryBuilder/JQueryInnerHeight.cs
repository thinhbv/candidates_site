namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryInnerHeight : JQuery
    {
        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            return ".innerHeight()";
        }
    }
}