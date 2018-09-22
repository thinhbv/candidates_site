using System.Collections.Generic;
using CMSSolutions.Caching;

namespace CMSSolutions.FileSystems.Dependencies
{
    public interface IDependenciesFolder : IVolatileProvider
    {
        DependencyDescriptor GetDescriptor(string moduleName);

        IEnumerable<DependencyDescriptor> LoadDescriptors();

        void StoreDescriptors(IEnumerable<DependencyDescriptor> dependencyDescriptors);
    }
}