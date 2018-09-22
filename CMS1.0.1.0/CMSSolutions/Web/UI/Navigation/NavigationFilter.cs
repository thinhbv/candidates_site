using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using CMSSolutions.DisplayManagement;
using CMSSolutions.Web.Mvc.Filters;
using CMSSolutions.Web.Themes;

namespace CMSSolutions.Web.UI.Navigation
{
    public class NavigationFilter : FilterProvider, IResultFilter
    {
        private readonly INavigationManager navigationManager;
        private readonly IWorkContextAccessor workContextAccessor;
        private readonly dynamic shapeFactory;

        public NavigationFilter(INavigationManager navigationManager, IWorkContextAccessor workContextAccessor, IShapeFactory shapeFactory)
        {
            this.shapeFactory = shapeFactory;
            this.navigationManager = navigationManager;
            this.workContextAccessor = workContextAccessor;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult))
            {
                return;
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                return;
            }

            var workContext = workContextAccessor.GetContext(filterContext);

            ThemedAttribute themedAttribute;
            if (!ThemeFilter.IsApplied(filterContext.RequestContext, out themedAttribute))
            {
                return;
            }

            if (!themedAttribute.IsDashboard)
            {
                return;
            }

            var menuItems = navigationManager.BuildMenu();
            if (menuItems.Count == 0)
            {
                return;
            }

            // Set the currently selected path
            SetSelectedPath(menuItems, filterContext.RouteData);

            // Populate main nav
            var menuShape = shapeFactory.Menu();
            menuShape.MenuItems = menuItems;

            workContext.Layout.Navigation.Add(menuShape);
        }

        /// <summary>
        /// Identifies the currently selected path, starting from the selected node.
        /// </summary>
        /// <param name="menuItems">All the menuitems in the navigation menu.</param>
        /// <param name="currentRouteData">The current route data.</param>
        /// <returns>A stack with the selection path being the last node the currently selected one.</returns>
        protected static Stack<MenuItem> SetSelectedPath(IEnumerable<MenuItem> menuItems, RouteData currentRouteData)
        {
            if (menuItems == null)
                return null;

            foreach (MenuItem menuItem in menuItems)
            {
                Stack<MenuItem> selectedPath = SetSelectedPath(menuItem.Items, currentRouteData);
                if (selectedPath != null)
                {
                    menuItem.Selected = true;
                    selectedPath.Push(menuItem);
                    return selectedPath;
                }

                if (RouteMatches(menuItem.RouteValues, currentRouteData.Values))
                {
                    menuItem.Selected = true;

                    selectedPath = new Stack<MenuItem>();
                    selectedPath.Push(menuItem);
                    return selectedPath;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines if a menu item corresponds to a given route.
        /// </summary>
        /// <param name="itemValues">The menu item.</param>
        /// <param name="requestValues">The route data.</param>
        /// <returns>True if the menu item's action corresponds to the route data; false otherwise.</returns>
        protected static bool RouteMatches(RouteValueDictionary itemValues, RouteValueDictionary requestValues)
        {
            if (itemValues == null && requestValues == null)
            {
                return true;
            }
            if (itemValues == null || requestValues == null)
            {
                return false;
            }
            if (itemValues.Keys.Any(key => requestValues.ContainsKey(key) == false))
            {
                return false;
            }
            return itemValues.Keys.All(key => string.Equals(Convert.ToString(itemValues[key]), Convert.ToString(requestValues[key]), StringComparison.OrdinalIgnoreCase));
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}