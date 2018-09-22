using System.Web.Routing;

namespace CMSSolutions.Web.Routing
{
    public interface IHasRequestContext
    {
        RequestContext RequestContext { get; }
    }
}