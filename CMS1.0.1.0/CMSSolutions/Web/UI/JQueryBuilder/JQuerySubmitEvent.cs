namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQuerySubmitEvent : JQuery
    {
        private readonly JQuery[] handlers;

        public JQuerySubmitEvent()
        {
        }

        public JQuerySubmitEvent(params JQuery[] handlers)
        {
            this.handlers = handlers;
        }

        public override string Build()
        {
            if (handlers == null || handlers.Length == 0)
            {
                return ".submit()";
            }

            return string.Format(".submit(function(){{ {0} }})", new JQueryHandlers(handlers));
        }
    }
}