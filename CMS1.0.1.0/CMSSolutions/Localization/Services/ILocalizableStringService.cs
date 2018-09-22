using System.Data.Entity.Core;
using CMSSolutions.Caching;
using CMSSolutions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization.Domain;
using CMSSolutions.Localization.Models;

namespace CMSSolutions.Localization.Services
{
    public interface ILocalizableStringService : IDependency
    {
        IList<ComparitiveLocalizableString> GetComparitiveTable(string keyword, string cultureCode, int pageIndex, int pageSize, out int totalRecords);

        void Update(string cultureCode, IList<ComparitiveLocalizableString> table);

        void Delete(string key);
    }

    [Feature(Constants.Areas.Localization)]
    public class LocalizableStringService : ILocalizableStringService
    {
        private readonly IRepository<LocalizableString, int> repository;
        private readonly ICacheManager cacheManager;

        public LocalizableStringService(IRepository<LocalizableString, int> repository, ICacheManager cacheManager)
        {
            this.repository = repository;
            this.cacheManager = cacheManager;
        }

        public virtual IList<ComparitiveLocalizableString> GetComparitiveTable(string keyword, string cultureCode, int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = repository.Table.Count(x => x.CultureCode == null);

            var table = new List<ComparitiveLocalizableString>();

            try
            {
                var query = repository.Table
                    .Where(x => x.TextKey.Contains(keyword) && (x.CultureCode == null || x.CultureCode == cultureCode))
                    .GroupBy(x => x.TextKey)
                    .Select(grp => new ComparitiveLocalizableString
                    {
                        Key = grp.Key,
                        InvariantValue = grp.FirstOrDefault(x => x.CultureCode == null).TextValue,
                        LocalizedValue = grp.FirstOrDefault(x => x.CultureCode == cultureCode) == null ? "" : grp.FirstOrDefault(x => x.CultureCode == cultureCode).TextValue
                    })
                    .OrderBy(x => x.Key)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize);

                table.AddRange(query.ToList());
            }
            catch (EntityCommandCompilationException)
            {
                //SQLite throws error: "APPLY joins are not supported"
                // So do it in memory instead

                var query = repository.Table
                     .Where(x => x.TextKey.Contains(keyword) && (x.CultureCode == null || x.CultureCode == cultureCode))
                    .ToHashSet()
                    .GroupBy(x => x.TextKey)
                    .Select(grp => new ComparitiveLocalizableString
                    {
                        Key = grp.Key,
                        InvariantValue = grp.First(x => x.CultureCode == null).TextValue,
                        LocalizedValue = grp.FirstOrDefault(x => x.CultureCode == cultureCode) == null ? "" : grp.First(x => x.CultureCode == cultureCode).TextValue
                    })
                    .OrderBy(x => x.Key)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize);

                table.AddRange(query.ToList());
            }

            return table;
        }

        public virtual void Update(string cultureCode, IList<ComparitiveLocalizableString> table)
        {
            var localizedStrings = repository.Table.Where(x => x.CultureCode == cultureCode).ToList().ToDictionary(k => k.TextKey, v => v.TextValue);
            var newItems = table.Where(x => !localizedStrings.Keys.Contains(x.Key));

            var inserts = newItems.Select(item => new LocalizableString
            {
                Id = 0,
                CultureCode = cultureCode,
                TextKey = item.Key,
                TextValue = item.LocalizedValue
            }).ToList();

            if (inserts.Any())
            {
                repository.InsertMany(inserts);
            }

            var changedItems = table.Where(x => localizedStrings.Keys.Contains(x.Key) && localizedStrings[x.Key] != x.LocalizedValue).ToList();
            var changedItemKeys = changedItems.Select(x => x.Key);

            var toUpdate = repository.Table
                .Where(x =>
                    x.CultureCode == cultureCode &&
                    changedItemKeys.Contains(x.TextKey))
                .ToList();

            var updates = new List<LocalizableString>();

            foreach (var item in toUpdate)
            {
                item.TextValue = changedItems.First(x => x.Key == item.TextKey).LocalizedValue;
                updates.Add(item);
            }

            if (updates.Any())
            {
                repository.UpdateMany(updates);
            }

            cacheManager.Reset();
        }

        public void Delete(string key)
        {
            var records = repository.Table.Where(x => x.TextKey == key).ToList();
            repository.DeleteMany(records);
            cacheManager.Reset();
        }
    }
}