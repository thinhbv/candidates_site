using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Autofac;

namespace CMSSolutions.Environment
{
    public class DefaultHostContainer : IHostContainer, IDependencyResolver
    {
        private readonly IContainer container;

        public DefaultHostContainer(IContainer container)
        {
            this.container = container;
        }

        private static bool TryResolveAtScope(ILifetimeScope scope, string key, Type serviceType, out object value)
        {
            if (scope == null)
            {
                value = null;
                return false;
            }
            return key == null ? scope.TryResolve(serviceType, out value) : scope.TryResolveKeyed(key, serviceType, out value);
        }

        private bool TryResolve(string key, Type serviceType, out object value)
        {
            return TryResolveAtScope(container, key, serviceType, out value);
        }

        private TService Resolve<TService>(Type serviceType, TService defaultValue = default(TService))
        {
            object value;
            return TryResolve(null, serviceType, out value) ? (TService)value : defaultValue;
        }

        TService IHostContainer.Resolve<TService>()
        {
            // Resolve service, or null
            return Resolve(typeof(TService), default(TService));
        }

        object IDependencyResolver.GetService(Type serviceType)
        {
            // Resolve service, or null
            return Resolve(serviceType, default(object));
        }

        IEnumerable<object> IDependencyResolver.GetServices(Type serviceType)
        {
            return Resolve<IEnumerable>(typeof(IEnumerable<>).MakeGenericType(serviceType), Enumerable.Empty<object>()).Cast<object>();
        }
    }
}