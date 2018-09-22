using System.Web.Mvc;
using Autofac;
using Autofac.Core.Registration;

namespace CMSSolutions.Localization
{
    public class LocalizationUtilities
    {
        public static Localizer Resolve(WorkContext workContext, string scope)
        {
            return workContext == null ? NullLocalizer.Instance : Resolve(workContext.Resolve<ILifetimeScope>(), scope);
        }

        public static Localizer Resolve(ControllerContext controllerContext, string scope)
        {
            WorkContext workContext = controllerContext.GetWorkContext();
            return Resolve(workContext, scope);
        }

        public static Localizer Resolve(IComponentContext context, string scope)
        {
            try
            {
                return context.Resolve<IText>(new NamedParameter("scope", scope)).Get;
            }
            catch (ComponentNotRegisteredException)
            {
                return NullLocalizer.Instance;
            }
        }
    }
}