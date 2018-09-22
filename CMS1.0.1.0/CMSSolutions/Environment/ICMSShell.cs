using System;
using System.Collections.Generic;
using Autofac.Features.OwnedInstances;
using Castle.Core.Logging;
using CMSSolutions.Tasks;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.Environment
{
    public interface ICMSShell
    {
        void Activate();

        void Terminate();
    }

    public class DefaultCMSShell : ICMSShell
    {
        private readonly IEnumerable<IRouteProvider> routeProviders;
        private readonly Func<Owned<IShellEvents>> eventsFactory;
        private readonly IRoutePublisher routePublisher;
        private readonly ISweepGenerator sweepGenerator;

        public DefaultCMSShell(
            IEnumerable<IRouteProvider> routeProviders,
            IRoutePublisher routePublisher,
            ISweepGenerator sweepGenerator,
            Func<Owned<IShellEvents>> eventsFactory)
        {
            this.routeProviders = routeProviders;
            this.routePublisher = routePublisher;
            this.sweepGenerator = sweepGenerator;
            this.eventsFactory = eventsFactory;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Activate()
        {
            var allRoutes = new List<RouteDescriptor>();
            foreach (var routeProvider in routeProviders)
            {
                routeProvider.GetRoutes(allRoutes);
            }

            routePublisher.Publish(allRoutes);

            using (var events = eventsFactory())
            {
                events.Value.Activated();
            }

            sweepGenerator.Activate();
        }

        public void Terminate()
        {
            SafelyTerminate(() =>
            {
                using (var events = eventsFactory())
                {
                    SafelyTerminate(() => events.Value.Terminating());
                }
            });

            SafelyTerminate(() => sweepGenerator.Terminate());
        }

        private void SafelyTerminate(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Logger.ErrorFormat(e, "An unexcepted error occured while terminating the Shell");
            }
        }
    }
}