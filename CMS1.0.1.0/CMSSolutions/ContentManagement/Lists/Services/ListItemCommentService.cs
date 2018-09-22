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
    public interface IListItemCommentService : IGenericService<ListComment, int>, IDependency
    {
        IEnumerable<ListComment> GetCommentsOfList(int listId, int pageIndex, int pageSize, out int totals);
    }

    [Feature(Constants.Areas.Lists)]
    public class ListItemCommentService : GenericService<ListComment, int>, IListItemCommentService
    {
        private readonly ISignals signals;

        public ListItemCommentService(IRepository<ListComment, int> repository, IEventBus eventBus, ISignals signals)
            : base(repository, eventBus)
        {
            this.signals = signals;
        }

        #region Overrides of GenericService<ListItemComment>

        protected override IOrderedQueryable<ListComment> MakeDefaultOrderBy(IQueryable<ListComment> queryable)
        {
            throw new NotImplementedException();
        }

        public override void Insert(ListComment record)
        {
            base.Insert(record);
            signals.Trigger("ListItems_Changed", record.ListItemId);
        }

        public override void Update(ListComment record)
        {
            Repository.Update(record);
            signals.Trigger("ListItems_Changed", record.ListItemId);
        }

        public override void Delete(ListComment record)
        {
            Repository.Delete(record);
        }

        public IEnumerable<ListComment> GetCommentsOfList(int listId, int pageIndex, int pageSize, out int totals)
        {
            totals = Repository.Table.Count();

            return Repository.Table
                .OrderByDescending(x => x.CreatedDate)
                .Where(x => x.ListId == listId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToList();
        }

        #endregion Overrides of GenericService<ListItemComment>
    }
}