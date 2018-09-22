using System.Text;

namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryFunction : JQuery
    {
        private readonly string name;
        private readonly string[] arguments;
        private readonly JQuery[] handlers;

        public JQueryFunction(params JQuery[] handlers)
        {
            this.handlers = handlers;
        }

        public JQueryFunction(string[] arguments, params JQuery[] handlers)
        {
            this.arguments = arguments;
            this.handlers = handlers;
        }

        public JQueryFunction(string name, params JQuery[] handlers)
        {
            this.name = name;
            this.handlers = handlers;
        }

        public JQueryFunction(string name, string[] arguments, params JQuery[] handlers)
        {
            this.name = name;
            this.arguments = arguments;
            this.handlers = handlers;
        }

        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(name))
            {
                if ((arguments == null || arguments.Length == 0) && (handlers == null || handlers.Length == 0))
                {
                    return name;
                }

                sb.AppendFormat("function {0}(", name);
            }
            else
            {
                sb.Append("function(");
            }

            if (arguments != null)
            {
                sb.Append(string.Join(", ", arguments));
            }

            sb.Append(")");

            sb.Append("{");

            if (handlers != null)
            {
                foreach (var handler in handlers)
                {
                    sb.Append(handler);
                }
            }

            sb.Append("}");

            if (!string.IsNullOrEmpty(name))
            {
                sb.Append(";");
            }
            return sb.ToString();
        }
    }
}