using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI
{
    public abstract class ResourceRegister
    {
        private readonly IResourcesManager resourcesManager;
        private readonly WorkContext workContext;

        protected ResourceRegister(WorkContext workContext)
        {
            resourcesManager = workContext.Resolve<IResourcesManager>();
            this.workContext = workContext;
        }

        protected abstract string VirtualBasePath { get; }

        protected abstract string ResourceType { get; }

        public virtual ResourceEntry Include(string path, bool isThemePath = false)
        {
            ResourceEntry resourceEntry;
            if (isThemePath)
            {
                var virtualBasePath = VirtualBasePath.Replace("~/", string.Empty);
                resourceEntry = new ResourceEntry(ResourceType, string.Format("~/Themes/{0}/{1}/{2}", workContext.CurrentTheme.Id, virtualBasePath, path));
            }
            else
            {
                resourceEntry = new ResourceEntry(ResourceType, string.Concat(VirtualBasePath, "/", path));
            }
            resourcesManager.RegisterResource(resourceEntry);
            resourceEntry.Location = ResourceLocation.Head;
            return resourceEntry;
        }

        public void IncludeExternal(string path)
        {
            resourcesManager.RegisterResource(new ResourceEntry(ResourceType, path));
        }

        public virtual void IncludeInline(string code, bool ignoreExists = false)
        {
            resourcesManager.RegisterInlineResource(ResourceType, code, ignoreExists);
        }

        public virtual MvcHtmlString Render(ResourceLocation location)
        {
            var resources = resourcesManager.GetResources(ResourceType, location);

            var inlineResources = location == ResourceLocation.Foot 
                ? resourcesManager.GetInlineResources(ResourceType) 
                : new List<string>();

            if (resources.Count == 0 && inlineResources.Count == 0)
            {
                return null;
            }

            var sb = new StringBuilder();

            foreach (var resource in resources)
            {
                sb.AppendLine(BuildResource(resource));
            }

            if (inlineResources.Count > 0)
            {
                sb.Append(BuildInlineResources(inlineResources));
            }

            return new MvcHtmlString(sb.ToString());
        }

        protected abstract string BuildResource(string url);
        
        protected abstract string BuildInlineResources(IEnumerable<string> resources);
    }
}