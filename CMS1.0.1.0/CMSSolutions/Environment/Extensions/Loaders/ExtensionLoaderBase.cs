using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CMSSolutions.Caching;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.FileSystems.Dependencies;

namespace CMSSolutions.Environment.Extensions.Loaders
{
    public abstract class ExtensionLoaderBase : IExtensionLoader
    {
        private readonly IDependenciesFolder dependenciesFolder;

        protected ExtensionLoaderBase(IDependenciesFolder dependenciesFolder)
        {
            this.dependenciesFolder = dependenciesFolder;
        }

        public abstract int Order { get; }

        public string Name { get { return GetType().Name; } }

        public virtual IEnumerable<ExtensionReferenceProbeEntry> ProbeReferences(ExtensionDescriptor descriptor)
        {
            return Enumerable.Empty<ExtensionReferenceProbeEntry>();
        }

        public virtual Assembly LoadReference(DependencyReferenceDescriptor reference)
        {
            return null;
        }

        public virtual bool IsCompatibleWithModuleReferences(ExtensionDescriptor extension, IEnumerable<ExtensionProbeEntry> references)
        {
            return true;
        }

        public abstract ExtensionProbeEntry Probe(ExtensionDescriptor descriptor);

        public virtual ExtensionEntry Load(ExtensionDescriptor descriptor)
        {
            var dependency = dependenciesFolder.GetDescriptor(descriptor.Id);
            if (dependency != null && dependency.LoaderName == Name)
            {
                return LoadWorker(descriptor);
            }
            return null;
        }

        public virtual void ReferenceActivated(ExtensionLoadingContext context, ExtensionReferenceProbeEntry referenceEntry)
        {
        }

        public virtual void ReferenceDeactivated(ExtensionLoadingContext context, ExtensionReferenceProbeEntry referenceEntry)
        {
        }

        public virtual void ExtensionActivated(ExtensionLoadingContext ctx, ExtensionDescriptor extension)
        {
        }

        public virtual void ExtensionDeactivated(ExtensionLoadingContext ctx, ExtensionDescriptor extension)
        {
        }

        public virtual void ExtensionRemoved(ExtensionLoadingContext ctx, DependencyDescriptor dependency)
        {
        }

        public virtual void Monitor(ExtensionDescriptor extension, Action<IVolatileToken> monitor)
        {
        }

        protected abstract ExtensionEntry LoadWorker(ExtensionDescriptor descriptor);

        public virtual IEnumerable<ExtensionCompilationReference> GetCompilationReferences(DependencyDescriptor dependency)
        {
            return Enumerable.Empty<ExtensionCompilationReference>();
        }

        public virtual IEnumerable<string> GetVirtualPathDependencies(DependencyDescriptor dependency)
        {
            return Enumerable.Empty<string>();
        }
    }
}