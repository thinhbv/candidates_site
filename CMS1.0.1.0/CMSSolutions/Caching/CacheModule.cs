using System;
using System.Linq;
using Autofac;
using Autofac.Core;

namespace CMSSolutions.Caching
{
    public class CacheModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultCacheManager>().As<ICacheManager>().InstancePerDependency();
            builder.RegisterType<CacheManager>().As<ICacheInfo>().InstancePerDependency();
            builder.RegisterType<StaticCacheManager>().As<IStaticCacheManager>().SingleInstance();
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            bool needsCacheManager = registration.Activator.LimitType
                .GetConstructors().Any(x => x.GetParameters().Any(xx => xx.ParameterType == typeof(ICacheManager)));

            if (needsCacheManager)
            {
                registration.Preparing += (sender, e) =>
                {
                    var parameter = new TypedParameter(typeof(ICacheManager), e.Context.Resolve<ICacheManager>(new TypedParameter(typeof(Type), registration.Activator.LimitType)));
                    e.Parameters = e.Parameters.Concat(new[] { parameter });
                };
            }
        }
    }
}