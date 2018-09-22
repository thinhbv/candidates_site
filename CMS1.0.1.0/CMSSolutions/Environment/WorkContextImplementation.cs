using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.Environment
{
    public class WorkContextImplementation : WorkContext
    {
        private readonly IComponentContext componentContext;
        private readonly ConcurrentDictionary<string, Func<object>> stateResolvers = new ConcurrentDictionary<string, Func<object>>();
        private readonly IEnumerable<IWorkContextStateProvider> workContextStateProviders;
        private readonly Breadcrumbs breadcrumbs;

        public WorkContextImplementation(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
            workContextStateProviders = componentContext.Resolve<IEnumerable<IWorkContextStateProvider>>();
            breadcrumbs = new Breadcrumbs();
        }

        public override T Resolve<T>()
        {
            return componentContext.Resolve<T>();
        }

        public override T ResolveOptional<T>()
        {
            return componentContext.ResolveOptional<T>();
        }

        public override T ResolveWithParameters<T>(params Parameter[] parameters)
        {
            return componentContext.Resolve<T>(parameters);
        }

        public override object Resolve(Type serviceType)
        {
            return componentContext.Resolve(serviceType);
        }

        public override T ResolveNamed<T>(string name)
        {
            return componentContext.ResolveNamed<T>(name);
        }

        public override bool TryResolve<T>(out T service)
        {
            return componentContext.TryResolve(out service);
        }

        public override T GetState<T>(string name)
        {
            var resolver = stateResolvers.GetOrAdd(name, FindResolverForState<T>);
            return (T)resolver();
        }

        private Func<object> FindResolverForState<T>(string name)
        {
            var resolver = workContextStateProviders.Select(wcsp => wcsp.Get<T>(name)).FirstOrDefault(value => !Equals(value, default(T)));

            if (resolver == null)
            {
                return () => default(T);
            }
            return () => resolver(this);
        }

        public override void SetState<T>(string name, T value)
        {
            stateResolvers[name] = () => value;
        }

        public override Breadcrumbs Breadcrumbs
        {
            get { return breadcrumbs; }
        }
    }
}