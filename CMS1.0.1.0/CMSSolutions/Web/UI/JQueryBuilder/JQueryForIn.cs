namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryForIn : JQuery
    {
        private readonly string variableName;
        private readonly JQuery obj;
        private readonly JQuery[] handlers;

        public JQueryForIn(string variableName, JQuery obj, params JQuery[] handlers)
        {
            this.variableName = variableName;
            this.obj = obj;
            this.handlers = handlers;
        }

        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            return string.Format("for({0} in {1}){{ {2} }}", variableName, obj, new JQueryHandlers(handlers));
        }
    }
}