using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Castle.DynamicProxy;

namespace CMSSolutions.Environment.AutofacUtil.DynamicProxy2
{
    public class DynamicProxyContext
    {
        private const string ProxyContextKey = "CMSSolutions.Environment.AutofacUtil.DynamicProxy2.DynamicProxyContext.ProxyContextKey";
        private const string InterceptorServicesKey = "CMSSolutions.Environment.AutofacUtil.DynamicProxy2.DynamicProxyContext.InterceptorServicesKey";

        private readonly IProxyBuilder proxyBuilder = new DefaultProxyBuilder();
        private readonly IDictionary<Type, Type> cache = new Dictionary<Type, Type>();

        /// <summary>
        /// Static method to resolve the context for a component registration. The context is set
        /// by using the registration builder extension method EnableDynamicProxy(context).
        /// </summary>
        public static DynamicProxyContext From(IComponentRegistration registration)
        {
            object value;
            if (registration.Metadata.TryGetValue(ProxyContextKey, out value))
                return value as DynamicProxyContext;
            return null;
        }

        /// <summary>
        /// Called indirectly from the EnableDynamicProxy extension method.
        /// Modifies a registration to support dynamic interception if needed, and act as a normal type otherwise.
        /// </summary>
        public void EnableDynamicProxy<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle>(
            IRegistrationBuilder<TLimit, TConcreteReflectionActivatorData, TRegistrationStyle> registrationBuilder)
            where TConcreteReflectionActivatorData : ConcreteReflectionActivatorData
        {
            // associate this context. used later by static DynamicProxyContext.From() method.
            registrationBuilder.WithMetadata(ProxyContextKey, this);

            // put a shim in place. this will return constructors for the proxy class if it interceptors have been added.
            registrationBuilder.ActivatorData.ConstructorFinder = new ConstructorFinderWrapper(
                registrationBuilder.ActivatorData.ConstructorFinder, this);

            // when component is being resolved, this even handler will place the array of appropriate interceptors as the first argument
            registrationBuilder.OnPreparing(e =>
            {
                object value;
                if (e.Component.Metadata.TryGetValue(InterceptorServicesKey, out value))
                {
                    var interceptorServices = (IEnumerable<Service>)value;
                    var interceptors = interceptorServices.Select(service => e.Context.ResolveService(service)).Cast<IInterceptor>().ToArray();
                    var parameter = new PositionalParameter(0, interceptors);
                    e.Parameters = new[] { parameter }.Concat(e.Parameters).ToArray();
                }
            });
        }

        /// <summary>
        /// Called indirectly from the InterceptedBy extension method.
        /// Adds services to the componenent's list of interceptors, activating the need for dynamic proxy
        /// </summary>
        public void AddInterceptorService(IComponentRegistration registration, Service service)
        {
            AddProxy(registration.Activator.LimitType);

            var interceptorServices = Enumerable.Empty<Service>();
            object value;
            if (registration.Metadata.TryGetValue(InterceptorServicesKey, out value))
            {
                interceptorServices = (IEnumerable<Service>)value;
            }

            registration.Metadata[InterceptorServicesKey] = interceptorServices.Concat(new[] { service }).Distinct().ToArray();
        }

        /// <summary>
        /// Ensures that a proxy has been generated for the particular type in this context
        /// </summary>
        public void AddProxy(Type type)
        {
            Type proxyType;
            if (cache.TryGetValue(type, out proxyType))
                return;

            lock (cache)
            {
                if (cache.TryGetValue(type, out proxyType))
                    return;

                cache[type] = proxyBuilder.CreateClassProxyType(type, null, ProxyGenerationOptions.Default);
            }
        }

        /// <summary>
        /// Determines if a proxy has been generated for the given type, and returns it.
        /// </summary>
        public bool TryGetProxy(Type type, out Type proxyType)
        {
            return cache.TryGetValue(type, out proxyType);
        }
    }
}