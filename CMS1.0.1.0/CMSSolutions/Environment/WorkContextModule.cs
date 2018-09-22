using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.Metadata;
using CMSSolutions.Environment.Extensions;
using Module = Autofac.Module;

namespace CMSSolutions.Environment
{
    [Feature(Constants.Areas.Core)]
    public class WorkContextModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WorkContextAccessor>()
                .As<IWorkContextAccessor>()
                .InstancePerMatchingLifetimeScope("shell");

            builder.Register(ctx => new WorkContextImplementation(ctx.Resolve<IComponentContext>()))
                .As<WorkContext>()
                .InstancePerMatchingLifetimeScope("work");

            builder.RegisterType<WorkContextProperty<HttpContextBase>>()
                .As<WorkContextProperty<HttpContextBase>>()
                .InstancePerMatchingLifetimeScope("work");

            builder.Register(ctx => ctx.Resolve<WorkContextProperty<HttpContextBase>>().Value)
                .As<HttpContextBase>()
                .InstancePerDependency();

            builder.RegisterGeneric(typeof(WorkValues<>))
                .InstancePerMatchingLifetimeScope("work");

            builder.RegisterSource(new WorkRegistrationSource());
        }
    }

    public class Work<T> where T : class
    {
        private readonly Func<Work<T>, T> resolve;

        public Work(Func<Work<T>, T> resolve)
        {
            this.resolve = resolve;
        }

        public T Value
        {
            get { return resolve(this); }
        }
    }

    internal class WorkValues<T> where T : class
    {
        public WorkValues(IComponentContext componentContext)
        {
            ComponentContext = componentContext;
            Values = new Dictionary<Work<T>, T>();
        }

        public IComponentContext ComponentContext { get; private set; }

        public IDictionary<Work<T>, T> Values { get; private set; }
    }

    /// <summary>
    /// Support the <see cref="Meta{T}"/>
    /// types automatically whenever type T is registered with the container.
    /// Metadata values come from the component registration's metadata.
    /// </summary>
    internal class WorkRegistrationSource : IRegistrationSource
    {
        private static readonly MethodInfo createMetaRegistrationMethod = typeof(WorkRegistrationSource).GetMethod(
            "CreateMetaRegistration", BindingFlags.Static | BindingFlags.NonPublic);

        private static bool IsClosingTypeOf(Type type, Type openGenericType)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == openGenericType;
        }

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            var swt = service as IServiceWithType;
            if (swt == null || !IsClosingTypeOf(swt.ServiceType, typeof(Work<>)))
                return Enumerable.Empty<IComponentRegistration>();

            var valueType = swt.ServiceType.GetGenericArguments()[0];

            var valueService = swt.ChangeType(valueType);

            var registrationCreator = createMetaRegistrationMethod.MakeGenericMethod(valueType);

            return registrationAccessor(valueService)
                .Select(v => registrationCreator.Invoke(null, new object[] { service, v }))
                .Cast<IComponentRegistration>();
        }

        public bool IsAdapterForIndividualComponents
        {
            get { return true; }
        }

        private static IComponentRegistration CreateMetaRegistration<T>(Service providedService, IComponentRegistration valueRegistration) where T : class
        {
            var rb = RegistrationBuilder.ForDelegate(
                (c, p) =>
                {
                    var workContextAccessor = c.Resolve<IWorkContextAccessor>();
                    return new Work<T>(w =>
                    {
                        var workContext = workContextAccessor.GetContext();
                        if (workContext == null)
                            return default(T);

                        var workValues = workContext.Resolve<WorkValues<T>>();

                        T value;
                        if (!workValues.Values.TryGetValue(w, out value))
                        {
                            value = (T)workValues.ComponentContext.ResolveComponent(valueRegistration, p);
                            workValues.Values[w] = value;
                        }
                        return value;
                    });
                })
                .As(providedService)
                .Targeting(valueRegistration);

            return rb.CreateRegistration();
        }
    }
}