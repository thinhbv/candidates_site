using System.ComponentModel;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using CMSSolutions.Configuration;
using CMSSolutions.Environment;
using CMSSolutions.Web;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStartCode), "Start")]

namespace CMSSolutions.Web
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class PreApplicationStartCode
    {
        private static bool startWasCalled;

        public static void Start()
        {
            if (!startWasCalled)
            {
                if (CMSConfigurationSection.Instance != null)
                {
                    DynamicModuleUtility.RegisterModule(typeof(WarmupHttpModule));
                    DynamicModuleUtility.RegisterModule(typeof(StaticFileModule));
                }
                startWasCalled = true;
            }
        }
    }
}