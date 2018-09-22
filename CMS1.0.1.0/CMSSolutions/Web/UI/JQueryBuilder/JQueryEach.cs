namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryEach : JQuery
    {
        private readonly JQuery[] handlers;

        public JQueryEach(params JQuery[] handlers)
        {
            this.handlers = handlers;
        }

        public override string Build()
        {
            return string.Format(".each(function(indexInArray, valueOfElement){{ {0} }})", new JQueryHandlers(handlers));
        }
    }
}