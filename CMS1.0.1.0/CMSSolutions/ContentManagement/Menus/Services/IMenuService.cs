using System;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.ContentManagement.Menus.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.Menus.Services
{
    public interface IMenuService : IGenericService<Menu, int>, IDependency
    {
        Menu GetMainMenu();
    }

    [Feature(Constants.Areas.Menus)]
    public class MenuService : GenericService<Menu, int>, IMenuService
    {
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;

        public MenuService(IRepository<Menu, int> repository, IEventBus eventBus, ISignals signals, ICacheManager cacheManager)
            : base(repository, eventBus)
        {
            this.signals = signals;
            this.cacheManager = cacheManager;
        }

        protected override IOrderedQueryable<Menu> MakeDefaultOrderBy(IQueryable<Menu> queryable)
        {
            return queryable.OrderBy(x => x.Name);
        }

        public override Menu GetById(int id)
        {
            return cacheManager.Get("Menus_GetById" + id, ctx =>
            {
                ctx.Monitor(signals.When("Menus_Changed"));

                return Repository.GetById(id);
            });
        }

        public override void Insert(Menu record)
        {
            base.Insert(record);
            signals.Trigger("Menus_Changed");
        }

        public override void Update(Menu record)
        {
            base.Update(record);
            signals.Trigger("Menus_Changed");
        }

        public override void Delete(Menu record)
        {
            base.Delete(record);
            signals.Trigger("Menus_Changed");
        }

        public Menu GetMainMenu()
        {
            return cacheManager.Get("Menus_GetMainMenu", ctx =>
            {
                ctx.Monitor(signals.When("Menus_Changed"));

                return GetRecord(x => x.IsMainMenu);
            });
        }
    }
}