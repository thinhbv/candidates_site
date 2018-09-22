namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryLiteral : JQuery
    {
        private readonly string text;

        public JQueryLiteral(string text)
        {
            this.text = text;
        }

        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            return text;
        }
    }
}