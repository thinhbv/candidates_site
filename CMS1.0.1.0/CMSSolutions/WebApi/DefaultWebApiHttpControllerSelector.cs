using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Autofac;
using Autofac.Core;
using Autofac.Features.Metadata;

namespace CMSSolutions.WebApi
{
    public class DefaultWebApiHttpControllerSelector : DefaultHttpControllerSelector
    {
        private readonly HttpConfiguration configuration;

        public DefaultWebApiHttpControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
            this.configuration = configuration;
        }

        protected bool TryResolve<T>(WorkContext workContext, object serviceKey, out T instance)
        {
            if (workContext != null && serviceKey != null)
            {
                var key = new KeyedService(serviceKey, typeof(T));
                object value;
                if (workContext.Resolve<ILifetimeScope>().TryResolveService(key, out value))
                {
                    instance = (T)value;
                    return true;
                }
            }

            instance = default(T);
            return false;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var routeData = request.GetRouteData();
            var areaName = routeData.GetAreaName();  
            var controllerName = base.GetControllerName(request);
            var serviceKey = (areaName + "/" + controllerName).ToLowerInvariant();  
            var controllerContext = new HttpControllerContext(configuration, routeData, request);
            Meta<Lazy<IHttpController>> info;
            var workContext = controllerContext.GetWorkContext();
            if (TryResolve(workContext, serviceKey, out info))
            {
                var type = (Type)info.Metadata["ControllerType"];

                return
                    new HttpControllerDescriptor(configuration, controllerName, type);
            }

            return null;
        }
    }
}