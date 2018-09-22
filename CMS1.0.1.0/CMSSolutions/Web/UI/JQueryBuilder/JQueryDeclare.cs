using System;

namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQueryDeclare : JQuery
    {
        private readonly string name;
        private readonly object value;

        public JQueryDeclare(string name, object value)
        {
            this.value = value;
            this.name = name;
        }

        public string Name { get { return name; } }

        protected override bool ReturnJQuery
        {
            get { return false; }
        }

        public override string Build()
        {
            if (string.IsNullOrEmpty(name))
            {
                if (value == null)
                {
                    return "null";
                }

                var str = value as string;
                if (str != null)
                {
                    return string.Format("\"{0}\"", JQueryUtility.EncodeJsString(str));
                }

                if (value is DateTime)
                {
                    var dt = (DateTime)value;
                    return string.Format("new Date({0})", JQueryUtility.GetUnixMilliseconds(dt));
                }

                return string.Format("{0}", value);
            }
            else
            {
                if (value == null)
                {
                    return string.Format("var {0} = null;", name);
                }

                var str = value as string;
                if (str != null)
                {
                    return string.Format("var {0} = \"{1}\";", name, JQueryUtility.EncodeJsString(str));
                }

                if (value is DateTime)
                {
                    var dt = (DateTime)value;
                    return string.Format("var {0} = new Date({1});", name, JQueryUtility.GetUnixMilliseconds(dt));
                }

                return string.Format("var {0} = {1};", name, value);
            }
        }
    }
}