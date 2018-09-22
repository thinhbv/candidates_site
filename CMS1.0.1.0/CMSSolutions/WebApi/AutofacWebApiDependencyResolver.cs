using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Autofac;

namespace CMSSolutions.WebApi
{
    public class AutofacWebApiDependencyResolver : IDependencyResolver
    {
        private readonly ILifetimeScope container;
        private readonly IDependencyScope rootDependencyScope;

        public AutofacWebApiDependencyResolver(ILifetimeScope container)
        {
            if (container == null) throw new ArgumentNullException("container");

            this.container = container;
            rootDependencyScope = new AutofacWebApiDependencyScope(container);
        }

        public ILifetimeScope Container
        {
            get { return container; }
        }

        public object GetService(Type serviceType)
        {
            return rootDependencyScope.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return rootDependencyScope.GetServices(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            ILifetimeScope lifetimeScope = container.BeginLifetimeScope();
            return new AutofacWebApiDependencyScope(lifetimeScope);
        }

        public void Dispose()
        {
            rootDependencyScope.Dispose();
        }
    }
}