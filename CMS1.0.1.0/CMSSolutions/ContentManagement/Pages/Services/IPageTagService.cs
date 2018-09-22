using System;
using System.Linq;
using CMSSolutions.ContentManagement.Pages.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.Pages.Services
{
    public interface IPageTagService : IGenericService<PageTag, int>, IDependency
    {
    }

    [Feature(Constants.Areas.Pages)]
    public class PageTagService : GenericService<PageTag, int>, IPageTagService
    {
        public PageTagService(IRepository<PageTag, int> repository, IEventBus eventBus) : base(repository, eventBus)
        {
        }

        protected override IOrderedQueryable<PageTag> MakeDefaultOrderBy(IQueryable<PageTag> queryable)
        {
            return queryable.OrderBy(x => x.Name);
        }
    }
}
