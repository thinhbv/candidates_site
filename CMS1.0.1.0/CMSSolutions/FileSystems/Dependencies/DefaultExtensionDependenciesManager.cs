using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Castle.Core.Logging;
using CMSSolutions.Caching;
using CMSSolutions.FileSystems.AppData;

namespace CMSSolutions.FileSystems.Dependencies
{
    /// <summary>
    /// Similar to "Dependencies.xml" file, except we also store "GetFileHash" result for every
    /// VirtualPath entry. This is so that if any virtual path reference in the file changes,
    /// the file stored by this component will also change.
    /// </summary>
    public class DefaultExtensionDependenciesManager : IExtensionDependenciesManager
    {
        private const string BasePath = "Dependencies";
        private const string FileName = "dependencies.compiled.xml";
        private readonly ICacheManager cacheManager;
        private readonly IAppDataFolder appDataFolder;
        private readonly InvalidationToken writeThroughToken;

        public DefaultExtensionDependenciesManager(ICacheManager cacheManager, IAppDataFolder appDataFolder)
        {
            this.cacheManager = cacheManager;
            this.appDataFolder = appDataFolder;
            writeThroughToken = new InvalidationToken();

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        private string PersistencePath
        {
            get { return appDataFolder.Combine(BasePath, FileName); }
        }

        public void StoreDependencies(IEnumerable<DependencyDescriptor> dependencyDescriptors, Func<DependencyDescriptor, string> fileHashProvider)
        {
            Logger.Info("Storing module dependency file.");

            var newDocument = CreateDocument(dependencyDescriptors, fileHashProvider);
            var previousDocument = ReadDocument(PersistencePath);
            if (XNode.DeepEquals(newDocument.Root, previousDocument.Root))
            {
                Logger.Debug("Existing document is identical to new one. Skipping save.");
            }
            else
            {
                WriteDocument(PersistencePath, newDocument);
            }

            Logger.Info("Done storing module dependency file.");
        }

        public IEnumerable<string> GetVirtualPathDependencies(string extensionId)
        {
            var descriptor = GetDescriptor(extensionId);
            if (descriptor != null && IsSupportedLoader(descriptor.LoaderName))
            {
                // Currently, we return the same file for every module. An improvement would be to return
                // a specific file per module (this would decrease the number of recompilations needed
                // when modules change on disk).
                yield return appDataFolder.GetVirtualPath(PersistencePath);
            }
        }

        public ActivatedExtensionDescriptor GetDescriptor(string extensionId)
        {
            return LoadDescriptors().FirstOrDefault(d => d.ExtensionId.Equals(extensionId, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<ActivatedExtensionDescriptor> LoadDescriptors()
        {
            return cacheManager.Get(PersistencePath, ctx =>
            {
                appDataFolder.CreateDirectory(BasePath);
                ctx.Monitor(appDataFolder.WhenPathChanges(ctx.Key));

                writeThroughToken.IsCurrent = true;
                ctx.Monitor(writeThroughToken);

                return ReadDescriptors(ctx.Key).ToList();
            });
        }

        private XDocument CreateDocument(IEnumerable<DependencyDescriptor> dependencies, Func<DependencyDescriptor, string> fileHashProvider)
        {
            Func<string, XName> ns = (name => XName.Get(name));

            var elements = dependencies
                .Where(dep => IsSupportedLoader(dep.LoaderName))
                .OrderBy(dep => dep.Name, StringComparer.OrdinalIgnoreCase)
                .Select(descriptor =>
                        new XElement(ns("Dependency"),
                            new XElement(ns("ExtensionId"), descriptor.Name),
                            new XElement(ns("LoaderName"), descriptor.LoaderName),
                            new XElement(ns("VirtualPath"), descriptor.VirtualPath),
                            new XElement(ns("Hash"), fileHashProvider(descriptor))));

            return new XDocument(new XElement(ns("Dependencies"), elements.ToArray()));
        }

        private IEnumerable<ActivatedExtensionDescriptor> ReadDescriptors(string persistancePath)
        {
            Func<string, XName> ns = (XName.Get);
            Func<XElement, string, string> elem = (e, name) => e.Element(ns(name)).Value;

            XDocument document = ReadDocument(persistancePath);
            return document
                .Elements(ns("Dependencies"))
                .Elements(ns("Dependency"))
                .Select(e => new ActivatedExtensionDescriptor
                {
                    ExtensionId = elem(e, "ExtensionId"),
                    VirtualPath = elem(e, "VirtualPath"),
                    LoaderName = elem(e, "LoaderName"),
                    Hash = elem(e, "Hash"),
                }).ToList();
        }

        private bool IsSupportedLoader(string loaderName)
        {
            //Note: this is hard-coded for now, to avoid adding more responsibilities to the IExtensionLoader
            //      implementations.
            return
                loaderName == "DynamicExtensionLoader" ||
                loaderName == "PrecompiledExtensionLoader";
        }

        private void WriteDocument(string persistancePath, XDocument document)
        {
            writeThroughToken.IsCurrent = false;
            using (var stream = appDataFolder.CreateFile(persistancePath))
            {
                document.Save(stream, SaveOptions.None);
                stream.Close();
            }
        }

        private XDocument ReadDocument(string persistancePath)
        {
            if (!appDataFolder.FileExists(persistancePath))
                return new XDocument();

            try
            {
                using (var stream = appDataFolder.OpenFile(persistancePath))
                {
                    return XDocument.Load(stream);
                }
            }
            catch (Exception e)
            {
                Logger.InfoFormat(e, "Error reading file '{0}'. Assuming empty.", persistancePath);
                return new XDocument();
            }
        }

        private class InvalidationToken : IVolatileToken
        {
            public bool IsCurrent { get; set; }
        }
    }
}