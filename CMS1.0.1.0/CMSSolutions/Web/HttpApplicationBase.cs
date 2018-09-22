using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using CMSSolutions.Copyrights;
using CMSSolutions.Environment;
using CMSSolutions.Environment.Extensions.Folders;
using CMSSolutions.Environment.Extensions.Loaders;
using CMSSolutions.Quartz;
using CMSSolutions.Quartz.Impl;
using CMSSolutions.Web.Security;
using CMSSolutions.Web.UI;

namespace CMSSolutions.Web
{
    public abstract class HttpApplicationBase : HttpApplication
    {
        private static Starter<ICMSHost> starter;
        private IScheduler scheduler;

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("Scripts/{*pathInfo}");
            routes.IgnoreRoute("Styles/{*pathInfo}");
            routes.IgnoreRoute("Media/{*pathInfo}");
            routes.IgnoreRoute("Images/{*pathInfo}");
            routes.IgnoreRoute("Content/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
        }

        protected void Application_BeginRequest()
        {
            starter.OnBeginRequest(this);
        }

        protected void Application_EndRequest()
        {
            starter.OnEndRequest(this);
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var ctx = HttpContext.Current;
            if (ctx.Request.IsAuthenticated)
            {
                ctx.User = new LazyGenericPrincipal(ctx.User, ctx.Request);
            }
        }

        protected void Application_Start()
        {
            var schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler();
            scheduler.Start();

            RegisterRoutes(RouteTable.Routes);
            starter = new Starter<ICMSHost>(HostInitialization, HostBeginRequest, HostEndRequest);
            starter.OnApplicationStart(this);
            OnApplicationStart();
            ResourcesManager.ResourcesLookup += LookupResources;
        }

        protected void Application_End(object sender, EventArgs e)
        {
            OnApplicationEnd();
        }

        protected virtual IEnumerable<string> GetDependencies()
        {
            return Enumerable.Empty<string>();
        }

        protected abstract IEnumerable<Type> GetExportedTypes();

        protected IEnumerable<Type> GetExportedTypesFromAssemblies(params Assembly[] assemblies)
        {
            return assemblies.SelectMany(x => x.GetExportedTypes()).ToList();
        }

        protected virtual void LookupResources(object sender, ResourcesLookupEventArgs e)
        {
            throw new NotSupportedException("This application does not support lookup resources.");
        }

        protected virtual IEnumerable<string> GetRequireJs(ResourceType resource)
        {
            return Enumerable.Empty<string>();
        } 

        protected virtual void OnApplicationEnd()
        {
        }

        protected virtual void OnApplicationStart()
        {
        }

        private static void HostBeginRequest(HttpApplication application, ICMSHost host)
        {
            application.Context.Items["originalHttpContext"] = application.Context;

            host.BeginRequest();
        }

        private static void HostEndRequest(HttpApplication application, ICMSHost host)
        {
            host.EndRequest();
        }

        private ICMSHost HostInitialization(HttpApplication application)
        {
            var host = CMSStarter.CreateHost(OnRegistrations);
            host.Initialize();
            host.BeginRequest();
            host.EndRequest();

            return host;
        }

        protected virtual void OnRegistrations(ContainerBuilder builder)
        {
            builder.Register(ctx => RouteTable.Routes).SingleInstance();
            builder.Register(ctx => ModelBinders.Binders).SingleInstance();
            builder.Register(ctx => ViewEngines.Engines).SingleInstance();

            if (scheduler != null)
            {
                builder.RegisterInstance(scheduler).As<IScheduler>().SingleInstance();
            }

            builder.RegisterInstance(new ApplicationFolders(GetDependencies)).As<IExtensionFolders>().SingleInstance();
            builder.RegisterInstance(new ApplicationExtensionLoader(GetExportedTypes)).As<IExtensionLoader>().SingleInstance();
        }
    }
}