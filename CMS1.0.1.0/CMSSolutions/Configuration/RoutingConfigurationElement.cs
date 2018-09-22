using System.Configuration;

namespace CMSSolutions.Configuration
{
    public class RoutingConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("dashboardBaseUrl", IsRequired = false, DefaultValue = "dashboard")]
        public string DashboardBaseUrl
        {
            get { return (string)base["dashboardBaseUrl"]; }
            set { base["dashboardBaseUrl"] = value; }
        }
    }
}