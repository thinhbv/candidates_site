using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using CMSSolutions.Localization;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.Web.UI.Navigation
{
    public class MenuItem
    {
        public MenuItem()
        {
            Permissions = Enumerable.Empty<Permission>();
        }

        public LocalizedString Text { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string Href { get; set; }

        public string Position { get; set; }

        public bool Selected { get; set; }

        public RouteValueDictionary RouteValues { get; set; }

        public IEnumerable<MenuItem> Items { get; set; }

        public IEnumerable<Permission> Permissions { get; set; }

        public string CssClass { get; set; }

        public string IconCssClass { get; set; }
    }
}