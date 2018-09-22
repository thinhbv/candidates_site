namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryClickEvent : JQuery
    {
        private readonly JQuery[] handlers;

        public JQueryClickEvent(params JQuery[] handlers)
        {
            this.handlers = handlers;
        }

        public override string Build()
        {
            return string.Format(".click(function(){{ {0} }})", new JQueryHandlers(handlers));
        }
    }
}