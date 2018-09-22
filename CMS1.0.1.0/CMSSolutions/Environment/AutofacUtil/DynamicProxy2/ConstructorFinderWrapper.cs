using System;
using System.Reflection;
using Autofac.Core.Activators.Reflection;

namespace CMSSolutions.Environment.AutofacUtil.DynamicProxy2
{
    internal class ConstructorFinderWrapper : IConstructorFinder
    {
        private readonly IConstructorFinder constructorFinder;
        private readonly DynamicProxyContext dynamicProxyContext;

        public ConstructorFinderWrapper(IConstructorFinder constructorFinder, DynamicProxyContext dynamicProxyContext)
        {
            this.constructorFinder = constructorFinder;
            this.dynamicProxyContext = dynamicProxyContext;
        }

        public ConstructorInfo[] FindConstructors(Type targetType)
        {
            Type proxyType;
            if (dynamicProxyContext.TryGetProxy(targetType, out proxyType))
            {
                return constructorFinder.FindConstructors(proxyType);
            }
            return constructorFinder.FindConstructors(targetType);
        }
    }
}