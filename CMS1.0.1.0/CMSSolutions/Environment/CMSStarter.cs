using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using Autofac;
using CMSSolutions.Caching;
using CMSSolutions.Environment.Configuration;
using CMSSolutions.Environment.Descriptor;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Folders;
using CMSSolutions.Environment.Extensions.Loaders;
using CMSSolutions.Environment.ShellBuilders;
using CMSSolutions.Environment.State;
using CMSSolutions.Events;
using CMSSolutions.Exceptions;
using CMSSolutions.FileSystems.AppData;
using CMSSolutions.FileSystems.Dependencies;
using CMSSolutions.FileSystems.LockFile;
using CMSSolutions.FileSystems.VirtualPath;
using CMSSolutions.FileSystems.WebSite;
using CMSSolutions.Logging;
using CMSSolutions.Services;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Mvc.DataAnnotations;
using CMSSolutions.Web.Mvc.ViewEngines.Razor;
using CMSSolutions.Web.Mvc.ViewEngines.ThemeAwareness;
using CMSSolutions.Web.UI.ControlForms;
using CMSSolutions.WebApi;

namespace CMSSolutions.Environment
{
    public static class CMSStarter
    {
        public static IContainer CreateHostContainer(Action<ContainerBuilder> registrations)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new CollectionOrderModule());
            builder.RegisterModule(new LoggingModule());
            builder.RegisterModule(new EventsModule());
            builder.RegisterModule(new CacheModule());

            // a single default host implementation is needed for bootstrapping a web app domain
            builder.RegisterType<DefaultEventBus>().As<IEventBus>().SingleInstance();
            builder.RegisterType<DefaultCacheHolder>().As<ICacheHolder>().SingleInstance();
            builder.RegisterType<DefaultCacheContextAccessor>().As<ICacheContextAccessor>().SingleInstance();
            builder.RegisterType<DefaultParallelCacheContext>().As<IParallelCacheContext>().SingleInstance();
            builder.RegisterType<DefaultAsyncTokenProvider>().As<IAsyncTokenProvider>().SingleInstance();
            builder.RegisterType<DefaultHostEnvironment>().As<IHostEnvironment>().SingleInstance();
            builder.RegisterType<DefaultHostLocalRestart>().As<IHostLocalRestart>().SingleInstance();
            builder.RegisterType<DefaultBuildManager>().As<IBuildManager>().SingleInstance();
            builder.RegisterType<ControlFormVirtualPathProvider>().As<ICustomVirtualPathProvider>().SingleInstance();
            builder.RegisterType<AppDataFolderRoot>().As<IAppDataFolderRoot>().SingleInstance();
            builder.RegisterType<DefaultRazorCompilationEvents>().As<IRazorCompilationEvents>().SingleInstance();
            builder.RegisterType<DefaultAssemblyLoader>().As<IAssemblyLoader>().SingleInstance();
            builder.RegisterType<AppDomainAssemblyNameResolver>().As<IAssemblyNameResolver>().SingleInstance();
            builder.RegisterType<GacAssemblyNameResolver>().As<IAssemblyNameResolver>().SingleInstance();
            builder.RegisterType<FrameworkAssemblyNameResolver>().As<IAssemblyNameResolver>().SingleInstance();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<ViewsBackgroundCompilation>().As<IViewsBackgroundCompilation>().SingleInstance();
            builder.RegisterType<DefaultExceptionPolicy>().As<IExceptionPolicy>().SingleInstance();
            builder.RegisterType<DefaultCriticalErrorProvider>().As<ICriticalErrorProvider>().SingleInstance();

            RegisterVolatileProvider<WebSiteFolder, IWebSiteFolder>(builder);
            RegisterVolatileProvider<AppDataFolder, IAppDataFolder>(builder);
            RegisterVolatileProvider<DefaultLockFileManager, ILockFileManager>(builder);
            RegisterVolatileProvider<Clock, IClock>(builder);
            RegisterVolatileProvider<DefaultDependenciesFolder, IDependenciesFolder>(builder);
            RegisterVolatileProvider<DefaultExtensionDependenciesManager, IExtensionDependenciesManager>(builder);
            RegisterVolatileProvider<DefaultAssemblyProbingFolder, IAssemblyProbingFolder>(builder);
            RegisterVolatileProvider<DefaultVirtualPathMonitor, IVirtualPathMonitor>(builder);
            RegisterVolatileProvider<DefaultVirtualPathProvider, IVirtualPathProvider>(builder);

            builder.RegisterType<DefaultCMSHost>().As<ICMSHost>().As<IEventHandler>().SingleInstance();
            {
                builder.RegisterType<ShellSettingsManager>().As<IShellSettingsManager>().SingleInstance();

                builder.RegisterType<ShellContextFactory>().As<IShellContextFactory>().SingleInstance();
                {
                    builder.RegisterType<ShellDescriptorCache>().As<IShellDescriptorCache>().SingleInstance();

                    builder.RegisterType<CompositionStrategy>().As<ICompositionStrategy>().SingleInstance();
                    {
                        builder.RegisterType<ShellContainerRegistrations>().As<IShellContainerRegistrations>().SingleInstance();
                        builder.RegisterType<ExtensionLoaderCoordinator>().As<IExtensionLoaderCoordinator>().SingleInstance();
                        builder.RegisterType<ExtensionMonitoringCoordinator>().As<IExtensionMonitoringCoordinator>().SingleInstance();
                        builder.RegisterType<ExtensionManager>().As<IExtensionManager>().SingleInstance();
                        {
                            builder.RegisterType<ExtensionHarvester>().As<IExtensionHarvester>().SingleInstance();
                            builder.RegisterType<FrameworkExtension>().As<IExtensionFolders>().SingleInstance();
                            builder.RegisterType<ModuleFolders>().As<IExtensionFolders>().SingleInstance()
                                .WithParameter(new NamedParameter("paths", new[] { "~/Modules" }));
                            builder.RegisterType<ThemeFolders>().As<IExtensionFolders>().SingleInstance()
                                .WithParameter(new NamedParameter("paths", new[] { "~/Themes" }));

                            builder.RegisterType<FrameworkExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                            builder.RegisterType<ReferencedExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                            //builder.RegisterType<PrecompiledExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                            builder.RegisterType<RawThemeExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                        }
                    }

                    builder.RegisterType<ShellContainerFactory>().As<IShellContainerFactory>().SingleInstance();
                }

                builder.RegisterType<DefaultProcessingEngine>().As<IProcessingEngine>().SingleInstance();
            }

            builder.RegisterType<RunningShellTable>().As<IRunningShellTable>().SingleInstance();
            builder.RegisterType<DefaultCMSShell>().As<ICMSShell>().InstancePerMatchingLifetimeScope("shell");

            registrations(builder);

            var optionalComponentsConfig = HostingEnvironment.MapPath("~/Config/HostComponents.config");
            if (File.Exists(optionalComponentsConfig))
                builder.RegisterModule(new HostComponentsConfigModule(optionalComponentsConfig));

            var container = builder.Build();

            //
            // Register Virtual Path Providers
            //
            if (HostingEnvironment.IsHosted)
            {
                foreach (var vpp in container.Resolve<IEnumerable<ICustomVirtualPathProvider>>())
                {
                    HostingEnvironment.RegisterVirtualPathProvider(vpp.Instance);
                }
            }

            ControllerBuilder.Current.SetControllerFactory(new CMSControllerFactory());

            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerSelector), new DefaultWebApiHttpControllerSelector(GlobalConfiguration.Configuration));
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new DefaultWebApiHttpHttpControllerActivator(GlobalConfiguration.Configuration));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ThemeAwareViewEngineShim());

            var hostContainer = new DefaultHostContainer(container);
            HostContainerRegistry.RegisterHostContainer(hostContainer);

            // Register localized data annotations
            ModelValidatorProviders.Providers.Clear();
            ModelValidatorProviders.Providers.Add(new LocalizedModelValidatorProvider());

            return container;
        }

        private static void RegisterVolatileProvider<TRegister, TService>(ContainerBuilder builder) where TService : IVolatileProvider
        {
            builder.RegisterType<TRegister>()
                .As<TService>()
                .As<IVolatileProvider>()
                .SingleInstance();
        }

        public static ICMSHost CreateHost(Action<ContainerBuilder> registrations)
        {
            var container = CreateHostContainer(registrations);
            return container.Resolve<ICMSHost>();
        }
    }
}