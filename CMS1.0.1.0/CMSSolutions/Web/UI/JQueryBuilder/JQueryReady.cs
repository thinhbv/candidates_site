namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryReady : JQuery
    {
        private readonly JQuery[] handlers;

        public JQueryReady(params JQuery[] handlers)
        {
            this.handlers = handlers;
        }

        public override string Build()
        {
            return string.Format("jQuery(document).ready(function(){{ {0} }});", new JQueryHandlers(handlers));
        }
    }
}