using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.Tasks
{
    [Feature(Constants.Areas.ScheduledTasks)]
    public class NavigationProvider : INavigationProvider
    {
        public NavigationProvider()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Configuration"),
                menu => menu.Add(T("Schedule Tasks"), "5", item => item
                    .Action("Index", "ScheduleTask", new { area = Constants.Areas.ScheduledTasks })
                    .IconCssClass("cx-icon cx-icon-schedule-tasks")
                    .Permission(TasksPermissions.ManageScheduleTasks)));
        }
    }
}