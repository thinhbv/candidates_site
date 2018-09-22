using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Castle.Core.Logging;
using CMSSolutions.Caching;
using CMSSolutions.Collections;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Extensions;
using CMSSolutions.FileSystems.WebSite;
using CMSSolutions.Localization;

namespace CMSSolutions.Environment.Extensions.Folders
{
    public class ExtensionHarvester : IExtensionHarvester
    {
        private const string NameSection = "name";
        private const string PathSection = "path";
        private const string DescriptionSection = "description";
        private const string VersionSection = "version";
        private const string AuthorSection = "author";
        private const string WebsiteSection = "website";
        private const string TagsSection = "tags";
        private const string AntiForgerySection = "antiforgery";
        private const string ZonesSection = "zones";
        private const string BaseThemeSection = "basetheme";
        private const string CategorySection = "category";
        private const string PrioritySection = "priority";
        private const string SessionStateSection = "sessionstate";

        private readonly ICacheManager cacheManager;
        private readonly IWebSiteFolder webSiteFolder;
        private readonly ICriticalErrorProvider criticalErrorProvider;
        private readonly IBuildManager buildManager;

        public ExtensionHarvester(ICacheManager cacheManager, IWebSiteFolder webSiteFolder, ICriticalErrorProvider criticalErrorProvider, IBuildManager buildManager)
        {
            this.cacheManager = cacheManager;
            this.webSiteFolder = webSiteFolder;
            this.criticalErrorProvider = criticalErrorProvider;
            this.buildManager = buildManager;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        public IEnumerable<ExtensionDescriptor> HarvestExtensions(IEnumerable<string> paths, string extensionType, string manifestName, bool manifestIsOptional)
        {
            return paths
                .SelectMany(path => HarvestExtensions(path, extensionType, manifestName, manifestIsOptional))
                .ToList();
        }

        private IEnumerable<ExtensionDescriptor> HarvestExtensions(string path, string extensionType, string manifestName, bool manifestIsOptional)
        {
            string key = string.Format("{0}-{1}-{2}", path, manifestName, extensionType);

            return cacheManager.Get(key, ctx =>
            {
                ctx.Monitor(webSiteFolder.WhenPathChanges(path));
                return AvailableExtensionsInFolder(path, extensionType, manifestName, manifestIsOptional).ToReadOnlyCollection();
            });
        }

        private List<ExtensionDescriptor> AvailableExtensionsInFolder(string path, string extensionType, string manifestName, bool manifestIsOptional)
        {
            Logger.InfoFormat("Start looking for extensions in '{0}'...", path);
            var subfolderPaths = webSiteFolder.ListDirectories(path);
            var localList = new List<ExtensionDescriptor>();
            foreach (var subfolderPath in subfolderPaths)
            {
                var extensionId = Path.GetFileName(subfolderPath.TrimEnd('/', '\\'));
                var manifestPath = Path.Combine(subfolderPath, manifestName);
                try
                {
                    var descriptor = GetExtensionDescriptor(path, extensionId, extensionType, manifestPath, manifestIsOptional);

                    if (descriptor == null)
                        continue;

                    if (descriptor.Path != null && !descriptor.Path.IsValidUrlSegment())
                    {
                        Logger.ErrorFormat("The module '{0}' could not be loaded because it has an invalid Path ({1}). It was ignored. The Path if specified must be a valid URL segment. The best bet is to stick with letters and numbers with no spaces.",
                                     extensionId,
                                     descriptor.Path);
                        criticalErrorProvider.RegisterErrorMessage(T("The extension '{0}' could not be loaded because it has an invalid Path ({1}). It was ignored. The Path if specified must be a valid URL segment. The best bet is to stick with letters and numbers with no spaces.",
                                     extensionId,
                                     descriptor.Path));
                        continue;
                    }

                    if (descriptor.Path == null)
                    {
                        descriptor.Path = descriptor.Name.IsValidUrlSegment()
                                              ? descriptor.Name
                                              : descriptor.Id;
                    }

                    localList.Add(descriptor);
                }
                catch (Exception ex)
                {
                    // Ignore invalid module manifests
                    Logger.ErrorFormat(ex, "The module '{0}' could not be loaded. It was ignored.", extensionId);
                    criticalErrorProvider.RegisterErrorMessage(T("The extension '{0}' manifest could not be loaded. It was ignored.", extensionId));
                }
            }
            Logger.InfoFormat("Done looking for extensions in '{0}': {1}", path, string.Join(", ", localList.Select(d => d.Id)));
            return localList;
        }

        public ExtensionDescriptor GetDescriptorForExtension(string locationPath, string extensionId, string extensionType, string manifestText)
        {
            Dictionary<string, string> manifest = ParseManifest(manifestText);
            var extensionDescriptor = new ExtensionDescriptor
            {
                Location = locationPath + "/" + extensionId,
                Id = extensionId,
                ExtensionType = extensionType,
                Name = GetValue(manifest, NameSection) ?? extensionId,
                Path = GetValue(manifest, PathSection),
                Description = GetValue(manifest, DescriptionSection),
                Version = GetValue(manifest, VersionSection),
                Author = GetValue(manifest, AuthorSection),
                WebSite = GetValue(manifest, WebsiteSection),
                Tags = GetValue(manifest, TagsSection),
                AntiForgery = GetValue(manifest, AntiForgerySection),
                Zones = GetValue(manifest, ZonesSection),
                BaseTheme = GetValue(manifest, BaseThemeSection),
                SessionState = GetValue(manifest, SessionStateSection)
            };
            extensionDescriptor.Features = GetFeaturesForExtension(extensionDescriptor);

            return extensionDescriptor;
        }

        private ExtensionDescriptor GetExtensionDescriptor(string locationPath, string extensionId, string extensionType, string manifestPath, bool manifestIsOptional)
        {
            return cacheManager.Get(manifestPath, context =>
            {
                context.Monitor(webSiteFolder.WhenPathChanges(manifestPath));
                var manifestText = webSiteFolder.ReadFile(manifestPath);
                if (manifestText == null)
                {
                    if (manifestIsOptional)
                    {
                        manifestText = string.Format("Id: {0}", extensionId);
                    }
                    else
                    {
                        return null;
                    }
                }

                return GetDescriptorForExtension(locationPath, extensionId, extensionType, manifestText);
            });
        }

        private static Dictionary<string, string> ParseManifest(string manifestText)
        {
            var manifest = new Dictionary<string, string>();

            using (StringReader reader = new StringReader(manifestText))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] field = line.Split(new[] { ":" }, 2, StringSplitOptions.None);
                    int fieldLength = field.Length;
                    if (fieldLength != 2)
                        continue;
                    for (int i = 0; i < fieldLength; i++)
                    {
                        field[i] = field[i].Trim();
                    }
                    switch (field[0].ToLowerInvariant())
                    {
                        case NameSection:
                            manifest.Add(NameSection, field[1]);
                            break;

                        case PathSection:
                            manifest.Add(PathSection, field[1]);
                            break;

                        case DescriptionSection:
                            manifest.Add(DescriptionSection, field[1]);
                            break;

                        case VersionSection:
                            manifest.Add(VersionSection, field[1]);
                            break;

                        case AuthorSection:
                            manifest.Add(AuthorSection, field[1]);
                            break;

                        case WebsiteSection:
                            manifest.Add(WebsiteSection, field[1]);
                            break;

                        case TagsSection:
                            manifest.Add(TagsSection, field[1]);
                            break;

                        case AntiForgerySection:
                            manifest.Add(AntiForgerySection, field[1]);
                            break;

                        case ZonesSection:
                            manifest.Add(ZonesSection, field[1]);
                            break;

                        case BaseThemeSection:
                            manifest.Add(BaseThemeSection, field[1]);
                            break;

                        case CategorySection:
                            manifest.Add(CategorySection, field[1]);
                            break;

                        case PrioritySection:
                            manifest.Add(PrioritySection, field[1]);
                            break;

                        case SessionStateSection:
                            manifest.Add(SessionStateSection, field[1]);
                            break;
                    }
                }
            }

            return manifest;
        }

        private IEnumerable<FeatureDescriptor> GetFeaturesForExtension(ExtensionDescriptor extensionDescriptor)
        {
            var featureDescriptors = new List<FeatureDescriptor>();

            if (extensionDescriptor.ExtensionType == "Module")
            {
                var assembly = buildManager.GetReferencedAssembly(extensionDescriptor.Id);
                if (assembly != null)
                {
                    var types = assembly.GetExportedTypes();
                    var featureProvider = typeof(IFeatureProvider);

                    var features = types.Where(x => x.IsClass && !x.IsAbstract && featureProvider.IsAssignableFrom(x))
                        .Select(x => (IFeatureProvider)Activator.CreateInstance(x))
                        .SelectMany(x => x.AvailableFeatures())
                        .ToList();

                    foreach (var feature in features)
                    {
                        feature.Extension = extensionDescriptor;
                        featureDescriptors.Add(feature);
                    }
                }
            }
            else if (extensionDescriptor.ExtensionType == "Theme")
            {
                var feature = new FeatureDescriptor
                {
                    Id = extensionDescriptor.Id,
                    Name = extensionDescriptor.Name,
                    Category = "Themes",
                    Extension = extensionDescriptor
                };
                featureDescriptors.Add(feature);
            }

            return featureDescriptors;
        }

        private static string GetValue(IDictionary<string, string> fields, string key)
        {
            string value;
            return fields.TryGetValue(key, out value) ? value : null;
        }
    }
}