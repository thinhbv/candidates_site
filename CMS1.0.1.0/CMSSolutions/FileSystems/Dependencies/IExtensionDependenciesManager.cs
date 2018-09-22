using System;
using System.Collections.Generic;
using CMSSolutions.Caching;

namespace CMSSolutions.FileSystems.Dependencies
{
    public interface IExtensionDependenciesManager : IVolatileProvider
    {
        void StoreDependencies(IEnumerable<DependencyDescriptor> dependencyDescriptors, Func<DependencyDescriptor, string> fileHashProvider);

        IEnumerable<string> GetVirtualPathDependencies(string extensionId);

        ActivatedExtensionDescriptor GetDescriptor(string extensionId);
    }
}