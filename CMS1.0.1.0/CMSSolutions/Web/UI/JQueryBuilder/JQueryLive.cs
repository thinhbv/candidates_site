namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryLive : JQuery
    {
        private readonly string events;
        private readonly JQuery[] handlers;

        public JQueryLive(string events, params JQuery[] handlers)
        {
            this.events = events;
            this.handlers = handlers;
        }

        public override string Build()
        {
            return string.Format(".live('{0}', function(){{ {1} }})", events, new JQueryHandlers(handlers));
        }
    }
}