using System;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.Configuration.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Extensions;
using CMSSolutions.Services;

namespace CMSSolutions.Configuration.Services
{
    public interface ISettingService : IDependency
    {
        TSettings GetSettings<TSettings>() where TSettings : ISettings, new();

        ISettings GetSettings(Type settingsType);

        void SaveSetting(string key, string value);

        void SaveSetting<TSettings>(TSettings settings) where TSettings : ISettings;
    }

    [Feature(Constants.Areas.Core)]
    public class DefaultSettingService : GenericService<Setting, int>, ISettingService
    {
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;

        public DefaultSettingService(IRepository<Setting, int> repository, IEventBus eventBus, ICacheManager cacheManager, ISignals signals)
            : base(repository, eventBus)
        {
            this.cacheManager = cacheManager;
            this.signals = signals;
        }

        public TSettings GetSettings<TSettings>() where TSettings : ISettings, new()
        {
            var key = typeof(TSettings).FullName;
            return cacheManager.Get(key, ctx =>
            {
                ctx.Monitor(signals.When("Settings_Changed"));
                var settings = GetRecords(x => x.Name == key).FirstOrDefault();
                if (settings == null || string.IsNullOrEmpty(settings.Value))
                {
                    return new TSettings();
                }

                return settings.Value.XmlDeserialize<TSettings>();
            });
        }

        public ISettings GetSettings(Type settingsType)
        {
            var key = settingsType.FullName;
            return cacheManager.Get(key, ctx =>
            {
                ctx.Monitor(signals.When("Settings_Changed"));
                var settings = GetRecords(x => x.Name == key).FirstOrDefault();
                if (settings == null || string.IsNullOrEmpty(settings.Value))
                {
                    return (ISettings)Activator.CreateInstance(settingsType);
                }

                return (ISettings)settings.Value.XmlDeserialize(settingsType);
            });
        }

        public void SaveSetting(string key, string value)
        {
            var setting = GetRecords(x => x.Name == key).FirstOrDefault();
            if (setting == null)
            {
                setting = new Setting { Name = key, Value = value };
                Insert(setting);
            }
            else
            {
                setting.Value = value;
                Update(setting);
            }
            signals.Trigger("Settings_Changed");
        }

        public void SaveSetting<TSettings>(TSettings settings) where TSettings : ISettings
        {
            var type = settings.GetType();
            var key = type.FullName;
            var value = settings.XmlSerialize();
            SaveSetting(key, value);
        }

        protected override IOrderedQueryable<Setting> MakeDefaultOrderBy(IQueryable<Setting> queryable)
        {
            return queryable.OrderBy(x => x.Name);
        }
    }
}