using System.Configuration;

namespace CMSSolutions.Configuration
{
    public class ModuleProviderCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public override bool IsReadOnly()
        {
            return false;
        } 

        protected override ConfigurationElement CreateNewElement()
        {
            return new ModuleProviderConfigurationElement();
        }

        public void Add(ModuleProviderConfigurationElement element)
        {
            BaseAdd(element);
        }

        public void Add(string id, string name, string category)
        {
            BaseAdd(new ModuleProviderConfigurationElement(id, name, category));
        }

        public void Remove(string id)
        {
            BaseRemove(id);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModuleProviderConfigurationElement)element).Id;
        }

        public new ModuleProviderConfigurationElement this[string id]
        {
            get
            {
                return (ModuleProviderConfigurationElement)BaseGet(id);
            }
        }
    }

    public class ModuleProviderConfigurationElement : ConfigurationElement
    {
        public ModuleProviderConfigurationElement()
        {
            
        }

        public ModuleProviderConfigurationElement(string id, string name, string category)
        {
            Id = id;
            Name = name;
            Category = category;
        }

        public override bool IsReadOnly()
        {
            return false;
        } 

        [ConfigurationProperty("id", IsRequired = true, IsKey = true)]
        public string Id
        {
            get { return (string)this["id"]; }
            set { this["id"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("category", IsRequired = false, DefaultValue = "Content")]
        public string Category
        {
            get { return (string)this["category"]; }
            set { this["category"] = value; }
        }
    }
}
