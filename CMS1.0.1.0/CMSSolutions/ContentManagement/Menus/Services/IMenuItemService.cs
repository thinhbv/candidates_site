using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.ContentManagement.Menus.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.Menus.Services
{
    public interface IMenuItemService : IGenericService<MenuItem, int>, IDependency
    {
        MenuItem GetMenuItemByRefId(int refId);

        IList<MenuItem> GetMenuItems(int menuId, bool enabledOnly = false);
    }

    [Feature(Constants.Areas.Menus)]
    public class MenuItemService : GenericService<MenuItem, int>, IMenuItemService
    {
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;

        public MenuItemService(IRepository<MenuItem, int> repository, 
            IEventBus eventBus, 
            ICacheManager cacheManager, 
            ISignals signals)
            : base(repository, eventBus)
        {
            this.cacheManager = cacheManager;
            this.signals = signals;
        }

        protected override IOrderedQueryable<MenuItem> MakeDefaultOrderBy(IQueryable<MenuItem> queryable)
        {
            return queryable.OrderBy(x => x.Position);
        }

        public override void Insert(MenuItem record)
        {
            base.Insert(record);

            signals.Trigger("MenuItems_Changed");
        }

        public override void Update(MenuItem record)
        {
            base.Update(record);

            signals.Trigger("MenuItems_Changed");
        }

        public override void Delete(MenuItem record)
        {
            base.Delete(record);

            signals.Trigger("MenuItems_Changed");
        }

        public MenuItem GetMenuItemByRefId(int refId)
        {
            return refId == 0
                ? null
                : Repository.Table.FirstOrDefault(x => x.RefId == refId);
        }

        public IList<MenuItem> GetMenuItems(int menuId, bool enabledOnly = false)
        {
            return cacheManager.Get("MenuItems_GetMenuItems" + menuId + "_" + enabledOnly, ctx =>
            {
                ctx.Monitor(signals.When("Menus_Changed"));
                ctx.Monitor(signals.When("MenuItems_Changed"));

                return enabledOnly
                    ? GetRecords(x => x.MenuId == menuId && x.Enabled).OrderBy(x => x.Position).ThenBy(x => x.Text).ToList()
                    : GetRecords(x => x.MenuId == menuId).OrderBy(x => x.Position).ThenBy(x => x.Text).ToList();
            });
        }
    }
}