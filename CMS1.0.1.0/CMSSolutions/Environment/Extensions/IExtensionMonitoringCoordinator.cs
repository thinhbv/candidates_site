using System;
using CMSSolutions.Caching;

namespace CMSSolutions.Environment.Extensions
{
    public interface IExtensionMonitoringCoordinator
    {
        void MonitorExtensions(Action<IVolatileToken> monitor);
    }
}