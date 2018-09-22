using System;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.ContentManagement.Pages.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.Pages.Services
{
    public interface IPageService : IGenericService<Page, int>, IDependency
    {
        Page GetPageBySlug(string slug);

        Page GetPageBySlug(string slug, string culture);

        void ToggleEnabled(int refId, bool isEnabled);

        Page GetPageByLanguage(int id, string cultureCode);
    }

    [Feature(Constants.Areas.Pages)]
    public class PageService : GenericService<Page, int>, IPageService
    {
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;

        public PageService(IRepository<Page, int> repository, 
            IEventBus eventBus, 
            ICacheManager cacheManager, 
            ISignals signals)
            : base(repository, eventBus)
        {
            this.cacheManager = cacheManager;
            this.signals = signals;
        }

        protected override IOrderedQueryable<Page> MakeDefaultOrderBy(IQueryable<Page> queryable)
        {
            return queryable.OrderBy(x => x.Title);
        }

        public void ToggleEnabled(int refId, bool isEnabled)
        {
            Repository.UpdateMany(x => x.Id == refId || x.RefId == refId, x => new Page { IsEnabled = isEnabled });
            signals.Trigger("Pages_Changed");
        }

        public Page GetPageByLanguage(int id, string cultureCode)
        {
            return Repository.Table.FirstOrDefault(x => x.RefId == id && x.CultureCode == cultureCode);
        }

        public override void Insert(Page record)
        {
            base.Insert(record);
            signals.Trigger("Pages_Changed");
        }

        public override void Update(Page record)
        {
            base.Update(record);

            if (record.RefId == null)
            {
                UpdateMany(x => x.RefId == record.Id, x => new Page
                    {
                        Slug = record.Slug,
                        Theme = record.Theme,
                        CssClass = record.CssClass
                    });
            }

            signals.Trigger("Pages_Changed");
        }

        public override void Delete(Page record)
        {
            base.Delete(record);
            signals.Trigger("Pages_Changed");
        }

        public Page GetPageBySlug(string slug)
        {
            return cacheManager.Get(BuildCacheKey("Pages_GetPageBySlug_" + slug), ctx =>
            {
                ctx.Monitor(signals.When("Pages_Changed"));
                return GetRecord(x => x.Slug == slug);
            });
        }

        public Page GetPageBySlug(string slug, string culture)
        {
            return cacheManager.Get(BuildCacheKey(string.Format("Pages_GetPageBySlug_{0}_{1}", slug, culture)), ctx =>
            {
                ctx.Monitor(signals.When("Pages_Changed"));
                return culture == null ?
                    GetRecord(x => x.Slug == slug && x.CultureCode == null) :
                    GetRecord(x => x.Slug == slug && x.CultureCode == culture);
            });
        }
    }
}