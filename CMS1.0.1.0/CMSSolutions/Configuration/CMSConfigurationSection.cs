using System.Configuration;

namespace CMSSolutions.Configuration
{
    public class CMSConfigurationSection : ConfigurationSection
    {
        private static CMSConfigurationSection instance;

        public static CMSConfigurationSection Instance
        {
            get
            {
                return instance ??
                       (instance = (CMSConfigurationSection)ConfigurationManager.GetSection("solutions"));
            }
        }

        [ConfigurationProperty("data", IsRequired = false)]
        public DataConfigurationElement Data
        {
            get { return (DataConfigurationElement)base["data"]; }
        }

        [ConfigurationProperty("routing", IsRequired = false)]
        public RoutingConfigurationElement Routing
        {
            get { return (RoutingConfigurationElement)base["routing"]; }
        }

        [ConfigurationProperty("modules")]
        [ConfigurationCollection(typeof(ModuleProviderCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public ModuleProviderCollection Modules
        {
            get { return (ModuleProviderCollection)this["modules"]; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}