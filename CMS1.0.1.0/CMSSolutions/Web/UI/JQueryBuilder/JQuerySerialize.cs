namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQuerySerialize : JQuery
    {
        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            return ".serialize()";
        }
    }
}