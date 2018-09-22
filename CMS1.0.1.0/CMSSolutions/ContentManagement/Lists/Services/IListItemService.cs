using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.Collections.Generic;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Linq;
using CMSSolutions.Linq.Dynamic;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.Lists.Services
{
    public interface IListItemService : IGenericService<ListItem, int>, IDependency
    {
        IList<ListItem> GetListItems(int listId, string ordering);

        IList<ListItem> GetListItems(int listId, string ordering, int pageIndex, int pageSize, out int totals);

        IList<ListItem> GetListItemsByCategoryId(int listId, int categoryId, string ordering, int pageIndex, int pageSize, out int totals);

        ListItem GetListItem(int listId, string slug, bool includeComments = false);

        void RemoveCategories(int itemId);

        void AddCategories(int itemId, int[] categories);
    }

    [Feature(Constants.Areas.Lists)]
    public class ListItemService : GenericService<ListItem, int>, IListItemService
    {
        private readonly IListCategoryService listCategoryService;
        private readonly IRepository<ListItemCategory, int> listItemCategoryRepository;
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;

        public ListItemService(IRepository<ListItem, int> repository, IRepository<ListItemCategory, int> listItemCategoryRepository, IEventBus eventBus, IListCategoryService listCategoryService, ICacheManager cacheManager, ISignals signals)
            : base(repository, eventBus)
        {
            this.cacheManager = cacheManager;
            this.signals = signals;
            this.listItemCategoryRepository = listItemCategoryRepository;
            this.listCategoryService = listCategoryService;
        }

        protected override IOrderedQueryable<ListItem> MakeDefaultOrderBy(IQueryable<ListItem> queryable)
        {
            return queryable.OrderBy(x => x.Position);
        }

        public override void Insert(ListItem record)
        {
            base.Insert(record);
            signals.Trigger("ListItems_Changed");
        }

        public override void Update(ListItem record)
        {
            Repository.Update(record);
            signals.Trigger("ListItems_Changed", record.Id);
        }

        public override void Delete(ListItem record)
        {
            Repository.Delete(record);
            signals.Trigger("ListItems_Changed", record.Id);
        }

        public IList<ListItem> GetListItems(int listId, string ordering)
        {
            return cacheManager.Get(string.Format("Lists_GetListItems_{0}_{1}", listId, ordering), ctx =>
            {
                ctx.Monitor(signals.When("ListItems_Changed"));

                if (string.IsNullOrEmpty(ordering))
                {
                    var items = Repository.Table
                        .Where(x => x.ListId == listId)
                        .OrderBy(x => x.Position)
                        .Select(x => x)
                        .ToList();

                    return items;
                }
                else
                {
                    var items = Repository.Table
                        .Include(x => x.Comments)
                        .Where(x => x.ListId == listId)
                        .OrderBy(ordering)
                        .Select(x => x)
                        .ToList();

                    return items;
                }
            });
        }

        public IList<ListItem> GetListItems(int listId, string ordering, int pageIndex, int pageSize, out int totals)
        {
            var list = cacheManager.Get(string.Format("Lists_GetListItems_{0}_{1}_{2}_{3}", listId, ordering, pageIndex, pageSize), ctx =>
            {
                ctx.Monitor(signals.When("ListItems_Changed"));

                int itemCount = Repository.Table.Count(x => x.ListId == listId);

                IPagedList<ListItem> result;

                if (string.IsNullOrEmpty(ordering))
                {
                    var items = Repository.Table
                        .Where(x => x.ListId == listId)
                        .OrderBy(x => x.Position)
                        .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    result = items.ToPagedList(pageIndex, pageSize, itemCount);
                }
                else
                {
                    var items = Repository.Table
                        .Include(x => x.Comments)
                        .Where(x => x.ListId == listId)
                        .OrderBy(ordering)
                        .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    result = items.ToPagedList(pageIndex, pageSize, itemCount);
                }

                return result;
            });

            totals = list.ItemCount;
            return list;
        }

        public IList<ListItem> GetListItemsByCategoryId(int listId, int categoryId, string ordering, int pageIndex, int pageSize, out int totals)
        {
            var categories = listCategoryService.GetCategories(listId);
            var childCategories = new List<int>();
            GetChildCategories(categoryId, categories, childCategories);
            childCategories.Insert(0, categoryId);

            var predicate = PredicateBuilder.False<ListItemCategory>();
            predicate = childCategories.Aggregate(predicate, (current, seed) => current.Or(x => x.CategoryId == seed));

            var query = from item in Repository.Table
                        join itemCategory in Repository.GetTable<ListItemCategory>().AsExpandable().Where(predicate)
                            on item.Id equals itemCategory.ItemId
                        select item;

            totals = query.Count();

            if (string.IsNullOrEmpty(ordering))
            {
                var items = query
                    .OrderBy(x => x.Position)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return items;
            }
            else
            {
                var items = query
                .OrderBy(ordering)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

                return items;
            }
        }

        private static void GetChildCategories(int categoryId, IList<ListCategory> categories, ICollection<int> childCategories)
        {
            var subCategories = categories.Where(x => x.ParentId == categoryId).ToList();

            foreach (var category in subCategories)
            {
                childCategories.Add(category.Id);
                GetChildCategories(category.Id, categories, childCategories);
            }
        }

        public ListItem GetListItem(int listId, string slug, bool includeComments = false)
        {
            return cacheManager.Get(string.Format("GetListItem_{0}_{1}_{2}", listId, includeComments, slug), ctx =>
            {
                var item = Repository.Table.Include(x => x.Comments, includeComments).FirstOrDefault(x => x.ListId == listId && x.Slug.ToLower() == slug.ToLower());

                if (item != null)
                {
                    ctx.Monitor(signals.When("ListItems_Changed", item.Id));
                }

                return item;
            });
        }

        public void RemoveCategories(int itemId)
        {
            var items = listItemCategoryRepository.Table.Where(x => x.ItemId == itemId).ToList();
            if (items.Count > 0)
            {
                listItemCategoryRepository.DeleteMany(items);
            }
        }

        public void AddCategories(int itemId, int[] categories)
        {
            var items = categories.Select(category => new ListItemCategory
                                                          {
                                                              Id = 0,
                                                              ItemId = itemId,
                                                              CategoryId = category
                                                          }).ToList();
            listItemCategoryRepository.InsertMany(items);
        }
    }
}