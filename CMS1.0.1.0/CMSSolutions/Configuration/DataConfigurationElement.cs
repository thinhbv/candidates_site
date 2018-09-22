using System.Configuration;

namespace CMSSolutions.Configuration
{
    public class DataConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("settingConnectionString", IsRequired = true)]
        public string SettingConnectionString
        {
            get { return (string)base["settingConnectionString"]; }
            set { base["settingConnectionString"] = value; }
        }

        [ConfigurationProperty("autoCreateTables", IsRequired = false, DefaultValue = false)]
        public bool AutoCreateTables
        {
            get { return (bool)base["autoCreateTables"]; }
            set { base["autoCreateTables"] = value; }
        }

        [ConfigurationProperty("providers")]
        [ConfigurationCollection(typeof(DataProviderCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public DataProviderCollection Providers
        {
            get { return (DataProviderCollection)this["providers"]; }
        }
    }

    public class DataProviderCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DataProviderConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DataProviderConfigurationElement)element).Name;
        }

        public new DataProviderConfigurationElement this[string name]
        {
            get
            {
                return (DataProviderConfigurationElement)BaseGet(name);
            }
        }
    }

    public class DataProviderConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("feature", IsRequired = true)]
        public string Feature
        {
            get { return (string)this["feature"]; }
            set { this["feature"] = value; }
        }
    }
}