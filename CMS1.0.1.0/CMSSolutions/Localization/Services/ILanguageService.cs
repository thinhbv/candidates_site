using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;

namespace CMSSolutions.Localization.Services
{
    public interface ILanguageService : IGenericService<Domain.Language, int>, IDependency
    {
        Domain.Language GetLanguage(string cultureCode);

        IEnumerable<Domain.Language> GetActiveLanguages();
    }

    [Feature(Constants.Areas.Localization)]
    public class LanguageService : GenericService<Domain.Language, int>, ILanguageService
    {
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;

        public LanguageService(IRepository<Domain.Language, int> repository, IEventBus eventBus, ISignals signals, ICacheManager cacheManager)
            : base(repository, eventBus)
        {
            this.signals = signals;
            this.cacheManager = cacheManager;
        }

        protected override IOrderedQueryable<Domain.Language> MakeDefaultOrderBy(IQueryable<Domain.Language> queryable)
        {
            return queryable.OrderBy(x => x.SortOrder);
        }

        public override void Insert(Domain.Language record)
        {
            base.Insert(record);
            signals.Trigger("Languages_Changed");
        }

        public override void Update(Domain.Language record)
        {
            base.Update(record);
            signals.Trigger("Languages_Changed");
        }

        public override void Delete(Domain.Language record)
        {
            base.Delete(record);
            signals.Trigger("Languages_Changed");
        }

        public Domain.Language GetLanguage(string cultureCode)
        {
            if (string.IsNullOrEmpty(cultureCode))
            {
                return null;
            }

            return cacheManager.Get("Culture_" + cultureCode, ctx =>
            {
                ctx.Monitor(signals.When("Languages_Changed"));

                var culture = Repository.Table.FirstOrDefault(x => x.CultureCode == cultureCode);
                if (culture == null)
                {
                    try
                    {
                        var parent = CultureInfo.GetCultureInfo(cultureCode);
                        var regionalLanguages = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(x => x.Parent.Equals(parent));
                        foreach (var language in regionalLanguages)
                        {
                            culture = Repository.Table.FirstOrDefault(x => x.CultureCode == language.Name);
                            if (culture == null) continue;
                            break;
                        }
                    }
                    catch (CultureNotFoundException)
                    {
                        culture = null;
                    }
                }

                return culture;
            });
        }

        public override IList<Domain.Language> GetRecords()
        {
            return cacheManager.Get("Languages_GetRecords", ctx =>
            {
                ctx.Monitor(signals.When("Languages_Changed"));

                return base.GetRecords();
            });
        }

        public IEnumerable<Domain.Language> GetActiveLanguages()
        {
            return cacheManager.Get("Languages_GetActiveLanguages", ctx =>
            {
                ctx.Monitor(signals.When("Languages_Changed"));

                return GetRecords(x => x.Active);
            });
        }
    }
}