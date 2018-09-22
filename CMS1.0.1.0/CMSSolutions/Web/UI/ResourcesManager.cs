using System;
using System.Collections.Generic;
using System.Linq;

namespace CMSSolutions.Web.UI
{
    public class ResourcesManager : IResourcesManager, IUnitOfWorkDependency
    {
        private readonly Dictionary<string, MetaEntry> metas = new Dictionary<string, MetaEntry>();

        private readonly Dictionary<string, ResourceEntry> resources =
            new Dictionary<string, ResourceEntry>(StringComparer.InvariantCultureIgnoreCase);

        private readonly Dictionary<string, string> inlineResources = new Dictionary<string, string>(); 

        #region IResourceManager Members

        public void RegisterResource(ResourceEntry resourceEntry)
        {
            if (!resources.ContainsKey(resourceEntry.Path))
            {
                resources.Add(resourceEntry.Path, resourceEntry);
            }
        }

        public void RegisterInlineResource(string type, string code, bool ignoreExists = false)
        {
            if (ignoreExists)
            {
                if (!inlineResources.ContainsKey(code))
                {
                    inlineResources.Add(code, type);    
                }
            }
            else
            {
                inlineResources.Add(code, type);                
            }
        }

        public IList<string> GetResources(string type, ResourceLocation location)
        {
            return resources
                .Where(x => x.Value.Type == type && x.Value.Location == location)
                .OrderBy(x => x.Value.Order)
                .Select(x => x.Key).ToList();
        }

        public IList<string> GetInlineResources(string type)
        {
            return inlineResources
                .Where(x => x.Value == type)
                .Select(x => x.Key).ToList();
        }

        public void SetMeta(MetaEntry meta)
        {
            metas[meta.Name] = meta;
        }

        public void AppendMeta(MetaEntry meta, string contentSeparator)
        {
            if (meta == null || String.IsNullOrEmpty(meta.Name))
            {
                return;
            }

            MetaEntry existingMeta;
            if (metas.TryGetValue(meta.Name, out existingMeta))
            {
                meta = MetaEntry.Combine(existingMeta, meta, contentSeparator);
            }
            metas[meta.Name] = meta;
        }

        public virtual IList<MetaEntry> GetRegisteredMetas()
        {
            return metas.Values.ToList().AsReadOnly();
        }

        #endregion IResourceManager Members

        #region Resources Lookup

        public static event EventHandler<ResourcesLookupEventArgs> ResourcesLookup;

        public static void LookupResources(ResourceType type, ScriptRegister script, StyleRegister style)
        {
            if (ResourcesLookup != null)
            {
                ResourcesLookup(null, new ResourcesLookupEventArgs(type, script, style));
            }
            else
            {
                throw new NotSupportedException("The application does not support lookup resources.");
            }
        }

        #endregion Resources Lookup
    }
}