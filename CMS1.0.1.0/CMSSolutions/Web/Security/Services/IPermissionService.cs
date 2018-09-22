using System;
using System.Linq;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;
using CMSSolutions.Web.Security.Domain;

namespace CMSSolutions.Web.Security.Services
{
    public interface IPermissionService : IGenericService<Permission, int>, IDependency
    {
        void DeleteByName(string name);
    }

    [Feature(Constants.Areas.Security)]
    public class PermissionService : GenericService<Permission, int>, IPermissionService
    {
        public PermissionService(IRepository<Permission, int> repository, IEventBus eventBus)
            : base(repository, eventBus)
        {
        }

        protected override IOrderedQueryable<Permission> MakeDefaultOrderBy(IQueryable<Permission> queryable)
        {
            return queryable.OrderBy(x => x.Name);
        }

        public void DeleteByName(string name)
        {
            var item = Repository.Table.FirstOrDefault(x => x.Name == name);
            if (item != null)
            {
                Delete(item, false); 
            }
        }
    }
}