using System;
using System.Linq;
using CMSSolutions.ContentManagement.Aliases.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.Aliases.Services
{
    public interface IAliasService : IGenericService<Alias, int>, IDependency
    {
    }

    [Feature(Constants.Areas.Aliases)]
    public class AliasService : GenericService<Alias, int>, IAliasService
    {
        public AliasService(IRepository<Alias, int> repository, IEventBus eventBus)
            : base(repository, eventBus)
        {
        }

        protected override IOrderedQueryable<Alias> MakeDefaultOrderBy(IQueryable<Alias> queryable)
        {
            return queryable.OrderBy(x => x.Path);
        }
    }
}