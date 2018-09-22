using System.Linq;
using System.Text;

namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryHandlers : JQuery
    {
        private readonly JQuery[] handlers;

        public JQueryHandlers(params JQuery[] handlers)
        {
            this.handlers = handlers;
        }

        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            if (handlers != null && handlers.Length > 0)
            {
                var sb = new StringBuilder();
                foreach (var handler in handlers.Where(handler => handler != null))
                {
                    sb.AppendLine(handler.ToString());
                }
                return sb.ToString();
            }

            return null;
        }
    }
}