using System.Linq;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.DisplayManagement;
using CMSSolutions.Localization;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.Security.Services;
using CMSSolutions.Web.UI;

namespace CMSSolutions.Web.Mvc
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>, IWebViewPage
    {
        private IResourcesManager resourcesManager;
        private IDisplayHelperFactory displayHelperFactory;
        private Localizer localizer = NullLocalizer.Instance;
        private bool initHelpers;

        public WorkContext WorkContext { get; private set; }

        public ScriptRegister Script { get; private set; }

        public StyleRegister Style { get; private set; }

        public dynamic Display { get; private set; }

        public new dynamic Layout { get; private set; }

        private IDisplayHelperFactory DisplayHelperFactory
        {
            get { return displayHelperFactory ?? (displayHelperFactory = WorkContext.Resolve<IDisplayHelperFactory>()); }
        }

        public Localizer T
        {
            get
            {
                // first time used, create it
                if (localizer == NullLocalizer.Instance)
                {
                    // not a shape, use the VirtualPath as scope
                    localizer = LocalizationUtilities.Resolve(ViewContext, VirtualPath);
                }

                return localizer;
            }
        }

        public override void InitHelpers()
        {
            base.InitHelpers();

            if (initHelpers)
            {
                return;
            }

            WorkContext = ViewContext.GetWorkContext();

            resourcesManager = WorkContext.Resolve<IResourcesManager>();

            Script = new ScriptRegister(WorkContext, this);

            Style = new StyleRegister(WorkContext);

            Display = DisplayHelperFactory.CreateHelper(ViewContext, this);

            Layout = WorkContext.Layout;

            initHelpers = true;
        }

        public void SetMeta(string name, string content)
        {
            SetMeta(new MetaEntry { Name = name, Content = content });
        }

        public virtual void SetMeta(MetaEntry meta)
        {
            resourcesManager.SetMeta(meta);
        }

        public void AppendMeta(string name, string content, string contentSeparator)
        {
            AppendMeta(new MetaEntry { Name = name, Content = content }, contentSeparator);
        }

        public virtual void AppendMeta(MetaEntry meta, string contentSeparator)
        {
            resourcesManager.AppendMeta(meta, contentSeparator);
        }

        public MvcHtmlString RenderMetas()
        {
            var metas = resourcesManager.GetRegisteredMetas();
            if (metas.Count == 0)
            {
                return null;
            }

            var sb = new StringBuilder();
            foreach (var meta in metas)
            {
                sb.Append(meta);
            }
            return new MvcHtmlString(sb.ToString());
        }

        public MvcHtmlString RenderHeadScripts()
        {
            return Script.Render(ResourceLocation.Head);
        }

        public MvcHtmlString RenderFootScripts()
        {
            return Script.Render(ResourceLocation.Foot);
        }

        public MvcHtmlString RenderStyles()
        {
            return Style.Render(ResourceLocation.Head);
        }

        public string Title(string separator, params object[] values)
        {
            return string.Join(separator, values.Where(x => x != null).ToArray());
        }

        public override void Execute()
        {
        }

        public bool IsUserInRole(string roleName)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return false;
            }

            var currentUser = WorkContext.CurrentUser;
            if (currentUser == null)
            {
                return false;
            }
            var membershipService = WorkContext.ResolveOptional<IMembershipService>();
            return membershipService != null && membershipService.IsUserInRole(currentUser.Id, roleName);
        }

        protected bool CheckPermission(Permission permission)
        {
            var authorizationService = WorkContext.Resolve<IAuthorizationService>();
            if (authorizationService.TryCheckAccess(permission, WorkContext.CurrentUser))
            {
                return true;
            }

            return false;
        }
    }

    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }

    public interface IWebViewPage : IViewDataContainer
    {
        Localizer T { get; }

        ScriptRegister Script { get; }

        StyleRegister Style { get; }
    }
}