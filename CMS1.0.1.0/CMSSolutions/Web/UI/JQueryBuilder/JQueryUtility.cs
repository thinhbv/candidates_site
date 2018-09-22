using System;
using System.Collections.Generic;
using System.Text;

namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public static class JQueryUtility
    {
        public static string EncodeJsString(IEnumerable<char> s, bool appendQuotes = true)
        {
            if (s == null)
            {
                return null;
            }

            var sb = new StringBuilder();

            if (appendQuotes)
            {
                sb.Append("\"");
            }

            foreach (char c in s)
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;

                    case '\\':
                        sb.Append("\\\\");
                        break;

                    case '\b':
                        sb.Append("\\b");
                        break;

                    case '\f':
                        sb.Append("\\f");
                        break;

                    case '\n':
                        sb.Append("\\n");
                        break;

                    case '\r':
                        sb.Append("\\r");
                        break;

                    case '\t':
                        sb.Append("\\t");
                        break;

                    default:
                        var i = (int)c;
                        if (i < 32 || i > 127)
                        {
                            sb.AppendFormat("\\u{0:X04}", i);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }

            if (appendQuotes)
            {
                sb.Append("\"");
            }

            return sb.ToString();
        }

        public static long GetUnixMilliseconds(DateTime dt)
        {
            return dt.Ticks - 621355968000000000;
        }
    }
}