using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using CMSSolutions.Web.Mvc.Filters;

namespace CMSSolutions.Web.Themes
{
    public class ThemeFilter : FilterProvider, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ThemedAttribute attribute;

            if (IsApplied(filterContext.RequestContext, out attribute))
            {
                return;
            }

            attribute = GetThemedAttribute(filterContext.ActionDescriptor);
            if (attribute != null && attribute.Enabled)
            {
                Apply(filterContext.RequestContext, attribute);
            }
        }

        public static void Apply(RequestContext context, ThemedAttribute attribute)
        {
            context.HttpContext.Items[typeof(ThemeFilter)] = attribute;
        }

        public static bool IsApplied(RequestContext context, out ThemedAttribute attribute)
        {
            if (context.HttpContext.Items.Contains(typeof(ThemeFilter)))
            {
                attribute = context.HttpContext.Items[typeof(ThemeFilter)] as ThemedAttribute;
                return true;
            }

            attribute = null;
            return false;
        }

        private static ThemedAttribute GetThemedAttribute(ActionDescriptor descriptor)
        {
            var actionAttribute = descriptor.GetCustomAttributes(typeof(ThemedAttribute), true).OfType<ThemedAttribute>().FirstOrDefault();
            var controllerAttribute = descriptor.ControllerDescriptor.GetCustomAttributes(typeof(ThemedAttribute), true).OfType<ThemedAttribute>().FirstOrDefault();

            if (actionAttribute == null)
            {
                return controllerAttribute;
            }

            if (controllerAttribute != null && !actionAttribute.IsDashboard)
            {
                actionAttribute.IsDashboard = controllerAttribute.IsDashboard;
            }

            return actionAttribute;
        }
    }
}