using System.Collections.Generic;

namespace CMSSolutions.Web.UI
{
    public interface IResourcesManager : IDependency
    {
        void RegisterResource(ResourceEntry resourceEntry);

        void RegisterInlineResource(string type, string code, bool ignoreExists = false);

        IList<string> GetResources(string type, ResourceLocation location);
        
        IList<string> GetInlineResources(string type);

        void SetMeta(MetaEntry meta);

        void AppendMeta(MetaEntry meta, string contentSeparator);

        IList<MetaEntry> GetRegisteredMetas();
    }
}