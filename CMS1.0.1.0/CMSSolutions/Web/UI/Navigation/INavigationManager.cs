using System.Collections.Generic;
using System.Web.Routing;

namespace CMSSolutions.Web.UI.Navigation
{
    public interface INavigationManager : IDependency
    {
        IList<MenuItem> BuildMenu();

        string GetUrl(string menuItemUrl, RouteValueDictionary routeValueDictionary);
    }
}