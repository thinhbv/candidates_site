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
    public class DefaultWebApiHttpHttpControllerActivator : IHttpControllerActivator
    {
        private readonly HttpConfiguration configuration;

        public DefaultWebApiHttpHttpControllerActivator(HttpConfiguration configuration)
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

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var routeData = request.GetRouteData();                    
            var controllerContext = new HttpControllerContext(configuration, routeData, request);  
            var areaName = routeData.GetAreaName();
            var serviceKey = (areaName + "/" + controllerDescriptor.ControllerName).ToLowerInvariant();
            Meta<Lazy<IHttpController>> info;
            var workContext = controllerContext.GetWorkContext();
            if (TryResolve(workContext, serviceKey, out info))
            {
                controllerContext.ControllerDescriptor =
                    new HttpControllerDescriptor(configuration, controllerDescriptor.ControllerName, controllerType);

                var controller = info.Value.Value;

                controllerContext.Controller = controller;

                return controller;
            }

            return null;
        }
    }
}