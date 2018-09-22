using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Aliases.Implementation.Holder
{
    [Feature(Constants.Areas.Aliases)]
    public class AliasInfo
    {
        public string Area { get; set; }

        public string Path { get; set; }

        public IDictionary<string, string> RouteValues { get; set; }
    }
}