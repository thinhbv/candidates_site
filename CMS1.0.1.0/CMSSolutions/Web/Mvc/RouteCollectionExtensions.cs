using System;
using System.Collections.Generic;
using System.Web.Routing;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.Web.Mvc
{
    public static class RouteCollectionExtensions
    {
        private static int topPosition = -1;

        public static void InsertToTop(this RouteCollection routes, RouteBase route)
        {
            EnsureTopPosition(routes);

            routes.Insert(topPosition, route);
        }

        public static void SortByPriority(this RouteCollection routes)
        {
            var items = new RouteBase[routes.Count];
            routes.CopyTo(items, 0);
            routes.Clear();

            Array.Sort(items, new RouteComparer());

            foreach (var item in items)
            {
                routes.Add(item);
            }
        }

        private static void EnsureTopPosition(IReadOnlyList<RouteBase> routes)
        {
            if (topPosition != -1) return;

            for (var index = 0; index < routes.Count; index++)
            {
                var route = routes[index] as ShellRoute;
                if (route != null)
                {
                    topPosition = index;
                    break;
                }
            }

            if (topPosition == -1)
            {
                topPosition = 0;
            }
        }

        private class RouteComparer : IComparer<RouteBase>
        {
            #region Implementation of IComparer<in RouteBase>

            public int Compare(RouteBase x, RouteBase y)
            {
                var routeX = x as ShellRoute;
                var routeY = y as ShellRoute;

                if (routeX == null && routeY == null)
                {
                    return 0;
                }

                if (routeX == null)
                {
                    return -1;
                }

                if (routeY == null)
                {
                    return 1;
                }

                throw new NotImplementedException();
                //return routeX.Priority.CompareTo(routeY.Priority);
            }

            #endregion Implementation of IComparer<in RouteBase>
        }
    }
}