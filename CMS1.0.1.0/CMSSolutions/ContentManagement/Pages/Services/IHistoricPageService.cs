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
    public interface IHistoricPageService : IGenericService<HistoricPage, int>, IDependency
    {
    }

    [Feature(Constants.Areas.Pages)]
    public class HistoricPageService : GenericService<HistoricPage, int>, IHistoricPageService
    {
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;
        private readonly PageSettings pageSettings;

        public HistoricPageService(
            IRepository<HistoricPage, int> repository,
            IEventBus eventBus,
            ICacheManager cacheManager,
            ISignals signals,
            PageSettings pageSettings)
            : base(repository, eventBus)
        {
            this.cacheManager = cacheManager;
            this.signals = signals;
            this.pageSettings = pageSettings;
        }

        protected override IOrderedQueryable<HistoricPage> MakeDefaultOrderBy(IQueryable<HistoricPage> queryable)
        {
            return queryable.OrderByDescending(x => x.ArchivedDate);
        }

        public override void Insert(HistoricPage record)
        {
            var pages = Repository.Table.Where(x => x.PageId == record.PageId);

            if (pages.Count() > (pageSettings.NumberOfPageVersionsToKeep - 1))
            {
                var pagesToKeep = pages
                    .OrderByDescending(x => x.ArchivedDate)
                    .Take(pageSettings.NumberOfPageVersionsToKeep - 1)
                    .Select(x => x.Id)
                    .ToList();

                var pagesToDelete = pages.Where(x => !pagesToKeep.Contains(x.Id));

                DeleteMany(pagesToDelete);
            }

            // now insert new record
            base.Insert(record);
            signals.Trigger("HistoricPages_Changed");
        }

        public override void Update(HistoricPage record)
        {
            base.Update(record);
            signals.Trigger("HistoricPages_Changed");
        }

        public override void Delete(HistoricPage record)
        {
            base.Delete(record);
            signals.Trigger("HistoricPages_Changed");
        }
    }
}