using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI
{
    public class StyleRegister : ResourceRegister
    {
        private readonly UrlHelper urlHelper;

        public StyleRegister(WorkContext workContext)
            : base(workContext)
        {
            urlHelper = workContext.Resolve<UrlHelper>();
        }

        protected override string VirtualBasePath
        {
            get { return "~/Styles"; }
        }

        protected override string ResourceType
        {
            get { return "text/css"; }
        }

        public override ResourceEntry Include(string path, bool isThemePath = false)
        {
            var entity = base.Include(path, isThemePath);
            entity.Location = ResourceLocation.Head;
            return entity;
        }

        public override void IncludeInline(string code, bool ignoreExists = false)
        {
            throw new NotSupportedException();
        }

        protected override string BuildResource(string url)
        {
            return string.Format("<link type=\"text/css\" rel=\"stylesheet\" href=\"{0}\" />", urlHelper.Content(url));
        }

        protected override string BuildInlineResources(IEnumerable<string> resources)
        {
            return string.Format("<style type=\"text/css\">{0}</style>", string.Join(System.Environment.NewLine, resources));
        }
    }
}