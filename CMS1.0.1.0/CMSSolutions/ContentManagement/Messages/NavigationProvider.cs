using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.ContentManagement.Messages
{
    [Feature(Constants.Areas.Messages)]
    public class NavigationProvider : INavigationProvider
    {
        private readonly ISmsSettings smsSettings;

        public NavigationProvider(ISmsSettings smsSettings = null)
        {
            this.smsSettings = smsSettings;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Configuration"),
                menu => menu.Add(T("Message Queue"), "5", item => item
                    .Action("Index", "QueuedEmail", new { area = Constants.Areas.Messages })
                    .IconCssClass("cx-icon cx-icon-message-queue")
                    .Permission(StandardPermissions.FullAccess)));
        }

        //private void BuildTemplatesMenu(NavigationItemBuilder builder)
        //{
        //    builder.Permission(StandardPermissions.FullAccess);
        //    builder.AddClass("fa fa-envelope");
        //    builder.Action("Index", "QueuedEmail", new { area = CMSConstants.Areas.Messages });
        //    builder.Add(T("Email Messages"), "1", item => item.Action("Index", "QueuedEmail", new { area = CMSConstants.Areas.Messages }));

        //    if (smsSettings != null)
        //    {
        //        builder.Add(T("SMS Messages"), "2", item => item.Action("Index", "QueuedSms", new { area = CMSConstants.Areas.Messages }));
        //    }

        //    builder.Add(T("Templates"), "3", item => item.Action("Index", "MessageTemplate", new { area = CMSConstants.Areas.Messages }));
        //}
    }
}