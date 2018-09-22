using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization.Domain;

namespace CMSSolutions.Localization
{
    [Feature(Constants.Areas.Localization)]
    public class DefaultLocalizedStringManager : ILocalizedStringManager
    {
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;
        private readonly IWorkContextAccessor workContextAccessor;
        private readonly object objSync = new object();

        public DefaultLocalizedStringManager(
            ICacheManager cacheManager,
            ISignals signals,
            IWorkContextAccessor workContextAccessor)
        {
            this.cacheManager = cacheManager;
            this.signals = signals;
            this.workContextAccessor = workContextAccessor;
        }

        #region ILocalizedStringManager Members

        public virtual string GetLocalizedString(string text, string cultureCode)
        {
            return GetResource(text, cultureCode);
        }

        #endregion ILocalizedStringManager Members

        protected virtual string GetResource(string key, string cultureCode)
        {
            lock (objSync)
            {
                var resourceCache = LoadCulture(cultureCode);

                if (resourceCache.ContainsKey(key))
                {
                    return resourceCache[key];
                }

                var invariantResourceCache = LoadCulture(null);

                if (invariantResourceCache.ContainsKey(key))
                {
                    return invariantResourceCache[key];
                }

                AddTranslation(null, key, key);

                invariantResourceCache.Add(key, key);
            }

            return key;
        }

        protected virtual IDictionary<string, string> LoadCulture(string cultureCode)
        {
            return cacheManager.Get("LocalizableString_" + cultureCode, ctx =>
            {
                ctx.Monitor(signals.When("LocalizableString_Changed"));
                return LoadTranslationsForCulture(cultureCode);
            });
        }

        protected virtual Dictionary<string, string> LoadTranslationsForCulture(string cultureCode)
        {
            var workContext = workContextAccessor.GetContext();
            var repository = workContext.Resolve<IRepository<LocalizableString, int>>();

            if (string.IsNullOrEmpty(cultureCode))
            {
                return LoadTranslations(repository.Table.Where(x => x.CultureCode == null).ToList());
            }

            return LoadTranslations(repository.Table.Where(x => x.CultureCode == cultureCode).ToList());
        }

        private static Dictionary<string, string> LoadTranslations(IEnumerable<LocalizableString> items)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var item in items.Where(item => !dictionary.ContainsKey(item.TextKey)))
            {
                dictionary.Add(item.TextKey, item.TextValue);
            }

            return dictionary;
        }

        protected virtual void AddTranslation(string cultureCode, string key, string value)
        {
            var workContext = workContextAccessor.GetContext();
            var repository = workContext.Resolve<IRepository<LocalizableString, int>>();
            repository.Insert(new LocalizableString
            {
                Id = 0,
                CultureCode = cultureCode,
                TextKey = key,
                TextValue = value
            });
        }
    }
}