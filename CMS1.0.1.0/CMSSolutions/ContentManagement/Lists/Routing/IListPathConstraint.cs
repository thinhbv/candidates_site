using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Lists.Routing
{
    public interface IListPathConstraint : IRouteConstraint, ISingletonDependency
    {
        void SetPaths(IDictionary<string, int> paths);

        void AddPath(string path, int id);

        void RemovePath(string path);
    }

    [Feature(Constants.Areas.Lists)]
    public class ListPathConstraint : IListPathConstraint
    {
        private readonly ConcurrentDictionary<string, int> paths = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (routeDirection == RouteDirection.UrlGeneration)
                return true;

            object value;
            if (values.TryGetValue(parameterName, out value))
            {
                var parameterValue = Convert.ToString(value);

                if (paths.ContainsKey(parameterValue))
                {
                    values.Add("listId", paths[parameterValue]);
                    return true;
                }
            }

            return false;
        }

        public void SetPaths(IDictionary<string, int> newPaths)
        {
            paths.Clear();
            foreach (var path in newPaths)
            {
                AddPath(path.Key, path.Value);
            }
        }

        public void AddPath(string path, int id)
        {
            // path can be null for homepage
            path = path ?? String.Empty;

            paths[path] = id;
        }

        public void RemovePath(string path)
        {
            // path can be null for homepage
            path = path ?? String.Empty;
            int id;
            paths.TryRemove(path, out id);
        }
    }
}