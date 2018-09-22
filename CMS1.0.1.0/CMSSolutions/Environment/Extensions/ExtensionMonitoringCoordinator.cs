using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using CMSSolutions.Caching;
using CMSSolutions.Environment.Extensions.Loaders;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.FileSystems.VirtualPath;

namespace CMSSolutions.Environment.Extensions
{
    public class ExtensionMonitoringCoordinator : IExtensionMonitoringCoordinator
    {
        private readonly IVirtualPathMonitor virtualPathMonitor;
        private readonly IAsyncTokenProvider asyncTokenProvider;
        private readonly IExtensionManager extensionManager;
        private readonly IEnumerable<IExtensionLoader> loaders;

        public ExtensionMonitoringCoordinator(
            IVirtualPathMonitor virtualPathMonitor,
            IAsyncTokenProvider asyncTokenProvider,
            IExtensionManager extensionManager,
            IEnumerable<IExtensionLoader> loaders)
        {
            this.virtualPathMonitor = virtualPathMonitor;
            this.asyncTokenProvider = asyncTokenProvider;
            this.extensionManager = extensionManager;
            this.loaders = loaders;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public bool Disabled { get; set; }

        public void MonitorExtensions(Action<IVolatileToken> monitor)
        {
            // We may be disabled by custom host configuration for performance reasons
            if (Disabled)
                return;

            //PERF: Monitor extensions asynchronously.
            monitor(asyncTokenProvider.GetToken(MonitorExtensionsWork));
        }

        public void MonitorExtensionsWork(Action<IVolatileToken> monitor)
        {
            Logger.Info("Start monitoring extension files...");
            // Monitor add/remove of any module/theme
            monitor(virtualPathMonitor.WhenPathChanges("~/Modules"));
            monitor(virtualPathMonitor.WhenPathChanges("~/Themes"));

            // Give loaders a chance to monitor any additional changes
            var extensions = extensionManager.AvailableExtensions().Where(d => DefaultExtensionTypes.IsModule(d.ExtensionType) || DefaultExtensionTypes.IsTheme(d.ExtensionType)).ToList();
            foreach (var extension in extensions)
            {
                foreach (var loader in loaders)
                {
                    loader.Monitor(extension, monitor);
                }
            }
            Logger.Info("Done monitoring extension files...");
        }
    }
}