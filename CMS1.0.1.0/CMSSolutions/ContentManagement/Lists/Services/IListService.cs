using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.Lists.Services
{
    public interface IListService : IGenericService<List, int>, IDependency
    {
        void EnableOrDisable(List record);

        bool IsUniqueUrl(int id, string url);
    }

    [Feature(Constants.Areas.Lists)]
    public class ListService : GenericService<List, int>, IListService
    {
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;

        public ListService(IRepository<List, int> repository, IEventBus eventBus, ICacheManager cacheManager, ISignals signals)
            : base(repository, eventBus)
        {
            this.cacheManager = cacheManager;
            this.signals = signals;
        }

        protected override IOrderedQueryable<List> MakeDefaultOrderBy(IQueryable<List> queryable)
        {
            return queryable.OrderBy(x => x.Name);
        }

        public override List GetById(int id)
        {
            return cacheManager.Get("Lists_GetById_" + id, ctx =>
            {
                ctx.Monitor(signals.When("Lists_Changed"));
                return Repository.Table.FirstOrDefault(x => x.Id == id);
            });
        }

        public override void Insert(List record)
        {
            base.Insert(record);
            signals.Trigger("Lists_Changed");
        }

        public override void Update(List record)
        {
            base.Update(record);
            signals.Trigger("Lists_Changed", record.Id);
        }

        public override void Delete(List record)
        {
            base.Delete(record);
            signals.Trigger("Lists_Changed", record.Id);
        }

        public void EnableOrDisable(List record)
        {
            if (record == null) return;
            record.Enabled = !record.Enabled;
            base.Update(record);
            signals.Trigger("Lists_Changed", record.Id);
        }

        public override IList<List> GetRecords()
        {
            return cacheManager.Get("GetLists", ctx =>
            {
                ctx.Monitor(signals.When("Lists_Changed"));

                return base.GetRecords();
            });
        }

        public bool IsUniqueUrl(int id, string url)
        {
            if (id != 0)
            {
                var list = Repository.Table.FirstOrDefault(x => x.Url == url);
                return list == null || list.Id == id;
            }

            return Repository.Table.FirstOrDefault(x => x.Url == url) == null;
        }
    }
}