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
    public interface IListCategoryService : IGenericService<ListCategory, int>, IDependency
    {
        IList<ListCategory> GetCategories(int listId);

        IList<int> GetItemCategories(int id);
    }

    [Feature(Constants.Areas.Lists)]
    public class ListCategoryService : GenericService<ListCategory, int>, IListCategoryService
    {
        private readonly IRepository<ListItemCategory, int> listItemCategoryRepository;
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;

        public ListCategoryService(IRepository<ListCategory, int> repository, IRepository<ListItemCategory, int> listItemCategoryRepository, IEventBus eventBus, ICacheManager cacheManager, ISignals signals)
            : base(repository, eventBus)
        {
            this.cacheManager = cacheManager;
            this.signals = signals;
            this.listItemCategoryRepository = listItemCategoryRepository;
        }

        protected override IOrderedQueryable<ListCategory> MakeDefaultOrderBy(IQueryable<ListCategory> queryable)
        {
            return queryable.OrderBy(x => x.Position);
        }

        public override void Insert(ListCategory record)
        {
            base.Insert(record);
            signals.Trigger("Categories_Changed");
        }

        public override void Update(ListCategory record)
        {
            Repository.Update(record);
            signals.Trigger("Categories_Changed");
        }

        public override void Delete(ListCategory record)
        {
            Repository.Delete(record);
            signals.Trigger("Categories_Changed");
        }

        public override ListCategory GetById(int id)
        {
            return cacheManager.Get("Categories_GetById_" + id, ctx =>
            {
                ctx.Monitor(signals.When("Categories_Changed"));

                return base.GetById(id);
            });
        }

        public IList<ListCategory> GetCategories(int listId)
        {
            return cacheManager.Get("Categories_GetCategories_" + listId, ctx =>
            {
                ctx.Monitor(signals.When("Categories_Changed"));

                return Repository.Table.Where(x => x.ListId == listId).OrderBy(x => x.Position).ToList();
            });
        }

        public IList<int> GetItemCategories(int id)
        {
            return listItemCategoryRepository.Table.Where(x => x.ItemId == id).Select(x => x.CategoryId).ToList();
        }
    }
}