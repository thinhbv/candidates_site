using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Autofac;

namespace CMSSolutions.WebApi
{
    public class AutofacWebApiDependencyScope : IDependencyScope
    {
        private readonly ILifetimeScope lifetimeScope;

        public AutofacWebApiDependencyScope(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public object GetService(Type serviceType)
        {
            return lifetimeScope.ResolveOptional(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (!lifetimeScope.IsRegistered(serviceType))
                return Enumerable.Empty<object>();

            var enumerableServiceType = typeof(IEnumerable<>).MakeGenericType(serviceType);
            var instance = lifetimeScope.Resolve(enumerableServiceType);
            return (IEnumerable<object>)instance;
        }

        public void Dispose()
        {
            if (lifetimeScope != null)
                lifetimeScope.Dispose();
        }
    }
}