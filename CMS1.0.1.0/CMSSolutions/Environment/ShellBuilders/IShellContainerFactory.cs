using System;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.Indexed;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.AutofacUtil.DynamicProxy2;
using CMSSolutions.Environment.ShellBuilders.Models;
using CMSSolutions.Events;

namespace CMSSolutions.Environment.ShellBuilders
{
    public interface IShellContainerFactory
    {
        ILifetimeScope CreateContainer(ShellSettings settings, ShellBlueprint blueprint);
    }

    public class ShellContainerFactory : IShellContainerFactory
    {
        private readonly ILifetimeScope lifetimeScope;
        private readonly IShellContainerRegistrations shellContainerRegistrations;

        public ShellContainerFactory(ILifetimeScope lifetimeScope, IShellContainerRegistrations shellContainerRegistrations)
        {
            this.lifetimeScope = lifetimeScope;
            this.shellContainerRegistrations = shellContainerRegistrations;
        }

        public ILifetimeScope CreateContainer(ShellSettings settings, ShellBlueprint blueprint)
        {
            var intermediateScope = lifetimeScope.BeginLifetimeScope(
                builder =>
                {
                    foreach (var item in blueprint.Dependencies.Where(t => typeof(IModule).IsAssignableFrom(t.Type)))
                    {
                        RegisterType(builder, item)
                            .Keyed<IModule>(item.Type)
                            .InstancePerDependency();
                    }
                });

            return intermediateScope.BeginLifetimeScope(
                "shell",
                builder =>
                {
                    var dynamicProxyContext = new DynamicProxyContext();

                    builder.Register(ctx => dynamicProxyContext);
                    builder.Register(ctx => settings);
                    builder.Register(ctx => blueprint.Descriptor);
                    builder.Register(ctx => blueprint);

                    var moduleIndex = intermediateScope.Resolve<IIndex<Type, IModule>>();
                    foreach (var item in blueprint.Dependencies.Where(t => typeof(IModule).IsAssignableFrom(t.Type)))
                    {
                        builder.RegisterModule(moduleIndex[item.Type]);
                    }

                    foreach (var item in blueprint.Dependencies.Where(t => typeof(IDependency).IsAssignableFrom(t.Type)))
                    {
                        var registration = RegisterType(builder, item)
                            .EnableDynamicProxy(dynamicProxyContext)
                            .InstancePerLifetimeScope();

                        foreach (var interfaceType in item.Type.GetInterfaces()
                            .Where(itf => typeof(IDependency).IsAssignableFrom(itf)
                                      && !typeof(IEventHandler).IsAssignableFrom(itf)))
                        {
                            registration = registration.As(interfaceType);
                            if (typeof(ISingletonDependency).IsAssignableFrom(interfaceType))
                            {
                                registration = registration.InstancePerMatchingLifetimeScope("shell");
                            }
                            else if (typeof(IUnitOfWorkDependency).IsAssignableFrom(interfaceType))
                            {
                                registration = registration.InstancePerMatchingLifetimeScope("work");
                            }
                            else if (typeof(ITransientDependency).IsAssignableFrom(interfaceType))
                            {
                                registration = registration.InstancePerDependency();
                            }
                            else if (typeof(IEntityTypeConfiguration).IsAssignableFrom(interfaceType))
                            {
                                if (item.Type.BaseType != null && item.Type.BaseType.GenericTypeArguments.Length == 1)
                                {
                                    var domainType = item.Type.BaseType.GenericTypeArguments[0];
                                    registration = registration.Named<IEntityTypeConfiguration>(domainType.Namespace);
                                }
                            }
                        }

                        if (typeof(IEventHandler).IsAssignableFrom(item.Type))
                        {
                            registration = registration.As(typeof(IEventHandler));
                        }
                    }

                    foreach (var item in blueprint.Controllers)
                    {
                        var serviceKeyName = (item.AreaName + "/" + item.ControllerName).ToLowerInvariant();
                        var serviceKeyType = item.Type;
                        RegisterType(builder, item)
                            .EnableDynamicProxy(dynamicProxyContext)
                            .Keyed<IController>(serviceKeyName)
                            .Keyed<IController>(serviceKeyType)
                            .WithMetadata("ControllerType", item.Type)
                            .InstancePerDependency()
                            .OnActivating(e =>
                            {
                                // necessary to inject custom filters dynamically see FilterResolvingActionInvoker
                                var controller = e.Instance as Controller;
                                if (controller != null)
                                    controller.ActionInvoker = (IActionInvoker)e.Context.ResolveService(new TypedService(typeof(IActionInvoker)));
                            });
                    }

                    foreach (var item in blueprint.HttpControllers)
                    {
                        var serviceKeyName = (item.AreaName + "/" + item.ControllerName).ToLowerInvariant();
                        var serviceKeyType = item.Type;
                        RegisterType(builder, item)
                            .EnableDynamicProxy(dynamicProxyContext)
                            .Keyed<IHttpController>(serviceKeyName)
                            .Keyed<IHttpController>(serviceKeyType)
                            .WithMetadata("ControllerType", item.Type)
                            .InstancePerDependency();
                    }

                    // Register code-only registrations specific to a shell
                    shellContainerRegistrations.Registrations(builder);
                });
        }

        private static IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType(ContainerBuilder builder, ShellBlueprintItem item)
        {
            return builder.RegisterType(item.Type)
                .WithProperty("Feature", item.Feature)
                .WithMetadata("Feature", item.Feature);
        }
    }
}