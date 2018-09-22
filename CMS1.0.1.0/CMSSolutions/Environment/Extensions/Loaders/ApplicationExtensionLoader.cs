using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CMSSolutions.Caching;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.FileSystems.Dependencies;

namespace CMSSolutions.Environment.Extensions.Loaders
{
    internal class ApplicationExtensionLoader : IExtensionLoader
    {
        private readonly Func<IEnumerable<Type>> getExportedTypes;

        public ApplicationExtensionLoader(Func<IEnumerable<Type>> getExportedTypes)
        {
            this.getExportedTypes = getExportedTypes;
        }

        public int Order { get { return 15; } }

        public string Name { get { return GetType().Name; } }

        public IEnumerable<ExtensionReferenceProbeEntry> ProbeReferences(ExtensionDescriptor extensionDescriptor)
        {
            return Enumerable.Empty<ExtensionReferenceProbeEntry>();
        }

        public Assembly LoadReference(DependencyReferenceDescriptor reference)
        {
            return null;
        }

        public void ReferenceActivated(ExtensionLoadingContext context, ExtensionReferenceProbeEntry referenceEntry)
        {
        }

        public void ReferenceDeactivated(ExtensionLoadingContext context, ExtensionReferenceProbeEntry referenceEntry)
        {
        }

        public bool IsCompatibleWithModuleReferences(ExtensionDescriptor extension, IEnumerable<ExtensionProbeEntry> references)
        {
            return true;
        }

        public ExtensionProbeEntry Probe(ExtensionDescriptor descriptor)
        {
            return null;
        }

        public ExtensionEntry Load(ExtensionDescriptor descriptor)
        {
            if (descriptor.Id == Constants.Areas.Application)
            {
                return new ExtensionEntry
                {
                    Descriptor = descriptor,
                    ExportedTypes = getExportedTypes()
                };
            }
            return null;
        }

        public void ExtensionActivated(ExtensionLoadingContext ctx, ExtensionDescriptor extension)
        {
        }

        public void ExtensionDeactivated(ExtensionLoadingContext ctx, ExtensionDescriptor extension)
        {
        }

        public void ExtensionRemoved(ExtensionLoadingContext ctx, DependencyDescriptor dependency)
        {
        }

        public void Monitor(ExtensionDescriptor extension, Action<IVolatileToken> monitor)
        {
        }

        public IEnumerable<ExtensionCompilationReference> GetCompilationReferences(DependencyDescriptor dependency)
        {
            return Enumerable.Empty<ExtensionCompilationReference>();
        }

        public IEnumerable<string> GetVirtualPathDependencies(DependencyDescriptor dependency)
        {
            return Enumerable.Empty<string>();
        }
    }
}