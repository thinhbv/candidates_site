using System.Collections.Generic;

namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryCallFunction : JQuery
    {
        private readonly string name;
        private readonly object[] args;

        public JQueryCallFunction(string name, params object[] args)
        {
            this.name = name;
            this.args = args;
        }

        public override string Build()
        {
            if (args == null || args.Length == 0)
            {
                return name + "()";
            }

            var list = new List<string>();
            foreach (var obj in args)
            {
                if (obj == null)
                {
                    list.Add("null");
                    continue;
                }

                var str = obj as string;
                if (str != null)
                {
                    list.Add("'" + str.Replace("'", "\'") + "'");
                    continue;
                }

                list.Add(obj.ToString());
            }

            return name + string.Format("({0})", string.Join(", ", list));
        }
    }
}