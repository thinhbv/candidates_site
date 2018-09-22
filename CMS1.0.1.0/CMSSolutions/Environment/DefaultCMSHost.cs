using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Castle.Core.Logging;
using CMSSolutions.Caching;
using CMSSolutions.Collections;
using CMSSolutions.Environment.Configuration;
using CMSSolutions.Environment.Descriptor;
using CMSSolutions.Environment.Descriptor.Models;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.ShellBuilders;
using CMSSolutions.Environment.State;
using CMSSolutions.Localization;

namespace CMSSolutions.Environment
{
    public class DefaultCMSHost : ICMSHost, IShellSettingsManagerEventHandler, IShellDescriptorManagerEventHandler
    {
        private readonly IHostLocalRestart hostLocalRestart;
        private readonly IShellSettingsManager shellSettingsManager;
        private readonly IShellContextFactory shellContextFactory;
        private readonly IRunningShellTable runningShellTable;
        private readonly IProcessingEngine processingEngine;
        private readonly IExtensionLoaderCoordinator extensionLoaderCoordinator;
        private readonly IExtensionMonitoringCoordinator extensionMonitoringCoordinator;
        private readonly ICacheManager cacheManager;
        private readonly object syncLock = new object();

        private IEnumerable<ShellContext> shellContexts;
        private IEnumerable<ShellSettings> tenantsToRestart;

        public DefaultCMSHost(
            IShellSettingsManager shellSettingsManager,
            IShellContextFactory shellContextFactory,
            IRunningShellTable runningShellTable,
            IProcessingEngine processingEngine,
            IExtensionLoaderCoordinator extensionLoaderCoordinator,
            IExtensionMonitoringCoordinator extensionMonitoringCoordinator,
            ICacheManager cacheManager,
            IHostLocalRestart hostLocalRestart)
        {
            this.shellSettingsManager = shellSettingsManager;
            this.shellContextFactory = shellContextFactory;
            this.runningShellTable = runningShellTable;
            this.processingEngine = processingEngine;
            this.extensionLoaderCoordinator = extensionLoaderCoordinator;
            this.extensionMonitoringCoordinator = extensionMonitoringCoordinator;
            this.cacheManager = cacheManager;
            this.hostLocalRestart = hostLocalRestart;
            tenantsToRestart = Enumerable.Empty<ShellSettings>();

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public int Priority { get { return 0; } }

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        public IList<ShellContext> Current
        {
            get { return BuildCurrent().ToReadOnlyCollection(); }
        }

        public ShellContext GetShellContext(ShellSettings shellSettings)
        {
            return Current
                .Single(shellContext => shellContext.Settings.Name.Equals(shellSettings.Name));
        }

        public ShellContext GetDefaultShellContext()
        {
            return Current
                .Single(shellContext => shellContext.Settings.Name.Equals("Default"));
        }

        void ICMSHost.Initialize()
        {
            Logger.Info("Initializing");
            BuildCurrent();
            Logger.Info("Initialized");
        }

        void ICMSHost.ReloadExtensions()
        {
            DisposeShellContext();
        }

        void ICMSHost.BeginRequest()
        {
            BeginRequest();
        }

        void ICMSHost.EndRequest()
        {
            EndRequest();
        }

        IWorkContextScope ICMSHost.CreateStandaloneEnvironment(ShellSettings shellSettings)
        {
            Logger.DebugFormat("Creating standalone environment for tenant {0}", shellSettings.Name);

            MonitorExtensions();
            BuildCurrent();
            var shellContext = CreateShellContext(shellSettings);
            return shellContext.LifetimeScope.CreateWorkContextScope();
        }

        /// <summary>
        /// Ensures shells are activated, or re-activated if extensions have changed
        /// </summary>
        private IEnumerable<ShellContext> BuildCurrent()
        {
            if (shellContexts == null)
            {
                lock (syncLock)
                {
                    if (shellContexts == null)
                    {
                        SetupExtensions();
                        MonitorExtensions();
                        CreateAndActivateShells();
                    }
                }
            }

            return shellContexts;
        }

        private void StartUpdatedShells()
        {
            lock (syncLock)
            {
                if (tenantsToRestart.Any())
                {
                    foreach (var settings in tenantsToRestart.Distinct().ToList())
                    {
                        ActivateShell(settings);
                    }

                    tenantsToRestart = Enumerable.Empty<ShellSettings>();
                }
            }
        }

        private void CreateAndActivateShells()
        {
            Logger.Info("Start creation of shells");

            var allSettings = shellSettingsManager.LoadSettings().ToArray();

            // load all tenants, and activate their shell
            if (allSettings.Any())
            {
                foreach (var settings in allSettings)
                {
                    try
                    {
                        var context = CreateShellContext(settings);
                        ActivateShell(context);
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorFormat(e, "A tenant could not be started: " + settings.Name);
#if DEBUG
                        throw;
#endif
                    }
                }
            }

            Logger.Info("Done creating shells");
        }

        /// <summary>
        /// Start a Shell and register its settings in RunningShellTable
        /// </summary>
        private void ActivateShell(ShellContext context)
        {
            Logger.DebugFormat("Activating context for tenant {0}", context.Settings.Name);
            context.Shell.Activate();

            shellContexts = (shellContexts ?? Enumerable.Empty<ShellContext>()).Union(new[] { context });
            runningShellTable.Add(context.Settings);

            // Startup tasks
            var startupTasks = context.LifetimeScope.Resolve<IEnumerable<IStartupTask>>();
            foreach (var startupTask in startupTasks)
            {
                try
                {
                    startupTask.Run();
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat(ex, "Have error when running startup task: " + startupTask);
                }
            }
        }

        private ShellContext CreateShellContext(ShellSettings settings)
        {
            Logger.DebugFormat("Creating shell context for tenant {0}", settings.Name);
            return shellContextFactory.CreateShellContext(settings);
        }

        private void SetupExtensions()
        {
            extensionLoaderCoordinator.SetupExtensions();
        }

        private void MonitorExtensions()
        {
            // This is a "fake" cache entry to allow the extension loader coordinator
            // notify us (by resetting _current to "null") when an extension has changed
            // on disk, and we need to reload new/updated extensions.
            cacheManager.Get("CMSHost_Extensions",
                              ctx =>
                              {
                                  extensionMonitoringCoordinator.MonitorExtensions(ctx.Monitor);
                                  hostLocalRestart.Monitor(ctx.Monitor);
                                  DisposeShellContext();
                                  return "";
                              });
        }

        /// <summary>
        /// Terminates all active shell contexts, and dispose their scope, forcing
        /// them to be reloaded if necessary.
        /// </summary>
        private void DisposeShellContext()
        {
            Logger.Info("Disposing active shell contexts");

            if (shellContexts != null)
            {
                foreach (var shellContext in shellContexts)
                {
                    shellContext.Shell.Terminate();
                    shellContext.LifetimeScope.Dispose();
                }
                shellContexts = null;
            }
        }

        protected virtual void BeginRequest()
        {
            // Ensure all shell contexts are loaded, or need to be reloaded if
            // extensions have changed
            MonitorExtensions();
            BuildCurrent();
            StartUpdatedShells();
        }

        protected virtual void EndRequest()
        {
            // Synchronously process all pending tasks. It's safe to do this at this point
            // of the pipeline, as the request transaction has been closed, so creating a new
            // environment and transaction for these tasks will behave as expected.)
            while (processingEngine.AreTasksPending())
            {
                processingEngine.ExecuteNextTask();
            }
        }

        /// <summary>
        /// Register and activate a new Shell when a tenant is created
        /// </summary>
        void IShellSettingsManagerEventHandler.Saved(ShellSettings settings)
        {
            lock (syncLock)
            {
                // if a tenant has been altered, and is not invalid, reload it
                if (settings.State != TenantState.Invalid)
                {
                    tenantsToRestart = tenantsToRestart.Where(x => x.Name != settings.Name).Union(new[] { settings });
                }
            }
        }

        private void ActivateShell(ShellSettings settings)
        {
            // look for the associated shell context
            var shellContext = shellContexts.FirstOrDefault(c => c.Settings.Name == settings.Name);

            // is this is a new tenant ? or is it a tenant waiting for setup ?
            if (shellContext == null)
            {
                // create the Shell
                var context = CreateShellContext(settings);

                // activate the Shell
                ActivateShell(context);
            }
            // reload the shell as its settings have changed
            else
            {
                // dispose previous context
                shellContext.Shell.Terminate();
                shellContext.LifetimeScope.Dispose();

                var context = shellContextFactory.CreateShellContext(settings);

                // activate and register modified context
                shellContexts = shellContexts.Where(shell => shell.Settings.Name != settings.Name).Union(new[] { context });

                context.Shell.Activate();

                runningShellTable.Update(settings);
            }
        }

        /// <summary>
        /// A feature is enabled/disabled
        /// </summary>
        void IShellDescriptorManagerEventHandler.Changed(ShellDescriptor descriptor, string tenant)
        {
            lock (syncLock)
            {
                if (shellContexts == null)
                {
                    return;
                }

                var context = shellContexts.FirstOrDefault(x => x.Settings.Name == tenant);

                // some shells might need to be started, e.g. created by command line
                if (context == null)
                {
                    StartUpdatedShells();
                    context = shellContexts.First(x => x.Settings.Name == tenant);
                }

                // don't update the settings themselves here
                if (tenantsToRestart.Any(x => x.Name == tenant))
                {
                    return;
                }

                tenantsToRestart = tenantsToRestart.Union(new[] { context.Settings });
            }
        }
    }
}