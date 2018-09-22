using System;
using System.Linq;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;
using CMSSolutions.Web.Security.Domain;

namespace CMSSolutions.Web.Security.Services
{
    public interface IRoleService : IGenericService<Role, int>, IDependency
    {
        Role GetRole(string name, bool createIfNotExist = false);
    }

    [Feature(Constants.Areas.Security)]
    public class RoleService : GenericService<Role, int>, IRoleService
    {
        public RoleService(IRepository<Role, int> repository, IEventBus eventBus)
            : base(repository, eventBus)
        {
        }

        protected override IOrderedQueryable<Role> MakeDefaultOrderBy(IQueryable<Role> queryable)
        {
            return queryable.OrderBy(x => x.Name);
        }

        public Role GetRole(string name, bool createIfNotExist = false)
        {
            var role = Repository.Table.FirstOrDefault(x => x.Name == name);
            if (role == null && createIfNotExist)
            {
                role = new Role
                           {
                               Name = name
                           };
                Insert(role);
            }

            return role;
        }
    }
}