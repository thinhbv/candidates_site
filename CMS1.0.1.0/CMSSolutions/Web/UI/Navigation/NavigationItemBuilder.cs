using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using CMSSolutions.Localization;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.Web.UI.Navigation
{
    public class NavigationItemBuilder : NavigationBuilder
    {
        private readonly MenuItem item;

        public NavigationItemBuilder()
        {
            item = new MenuItem();
        }

        public NavigationItemBuilder Caption(LocalizedString caption)
        {
            item.Text = caption;
            return this;
        }

        public NavigationItemBuilder Position(string position)
        {
            item.Position = position;
            return this;
        }

        public NavigationItemBuilder Url(string url)
        {
            item.Url = url;
            return this;
        }

        public NavigationItemBuilder Description(string description)
        {
            item.Description = description;
            return this;
        }

        public NavigationItemBuilder CssClass(string className)
        {
            item.CssClass = className;
            return this;
        }

        public NavigationItemBuilder IconCssClass(string className)
        {
            item.IconCssClass = className;
            return this;
        }

        public override IEnumerable<MenuItem> Build()
        {
            item.Items = base.Build();
            return new[] { item };
        }

        public NavigationItemBuilder Action(RouteValueDictionary values)
        {
            return values != null
                       ? Action(values["action"] as string, values["controller"] as string, values)
                       : Action(null, null, new RouteValueDictionary());
        }

        public NavigationItemBuilder Action(string actionName)
        {
            return Action(actionName, null, new RouteValueDictionary());
        }

        public NavigationItemBuilder Action(string actionName, string controllerName)
        {
            return Action(actionName, controllerName, new RouteValueDictionary());
        }

        public NavigationItemBuilder Action(string actionName, string controllerName, object values)
        {
            return Action(actionName, controllerName, new RouteValueDictionary(values));
        }

        public NavigationItemBuilder Action(string actionName, string controllerName, RouteValueDictionary values)
        {
            item.RouteValues = new RouteValueDictionary(values);
            if (!string.IsNullOrEmpty(actionName))
                item.RouteValues["action"] = actionName;
            if (!string.IsNullOrEmpty(controllerName))
                item.RouteValues["controller"] = controllerName;
            return this;
        }

        public NavigationItemBuilder Permission(params Permission[] permissions)
        {
            item.Permissions = item.Permissions.Concat(permissions);
            return this;
        }
    }
}