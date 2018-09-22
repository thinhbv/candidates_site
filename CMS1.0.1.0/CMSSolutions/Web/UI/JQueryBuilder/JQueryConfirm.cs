namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryConfirm : JQuery
    {
        private readonly string message;
        private readonly bool endStatement;

        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public JQueryConfirm(string message, bool endStatement = false)
        {
            this.message = message;
            this.endStatement = endStatement;
        }

        public override string Build()
        {
            return string.Format("confirm('{0}'){1}", message.Replace("'", "\'"), endStatement ? ";" : "");
        }
    }
}