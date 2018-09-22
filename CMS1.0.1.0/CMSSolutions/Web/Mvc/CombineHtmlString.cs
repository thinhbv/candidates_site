using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSSolutions.Web.Mvc
{
    public class CombineHtmlString : IHtmlString
    {
        private readonly string htmlString;

        public CombineHtmlString(params object[] fragments)
        {
            htmlString = fragments.Where(x => x != null).Aggregate("", (a, b) => a + b);
        }

        public CombineHtmlString(params IHtmlString[] fragments)
        {
            htmlString = fragments.Where(x => x != null).Aggregate("", (a, b) => a + b);
        }

        public CombineHtmlString(IEnumerable<IHtmlString> fragments)
        {
            htmlString = fragments.Where(x => x != null).Aggregate("", (a, b) => a + b);
        }

        public string ToHtmlString()
        {
            return htmlString;
        }

        public override string ToString()
        {
            return htmlString;
        }
    }
}