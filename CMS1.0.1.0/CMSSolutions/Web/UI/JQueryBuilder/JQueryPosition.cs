namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryPosition : JQuery
    {
        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            return ".position()";
        }
    }
}