using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.ContentManagement.Widgets.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.Widgets.Services
{
    public interface IZoneService : IGenericService<Zone, int>, IDependency
    {
        Zone GetByName(string name);
    }

    [Feature(Constants.Areas.Widgets)]
    public class ZoneService : GenericService<Zone, int>, IZoneService
    {
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;

        public ZoneService(IRepository<Zone, int> repository, 
            IEventBus eventBus, 
            ICacheManager cacheManager, 
            ISignals signals)
            : base(repository, eventBus)
        {
            this.cacheManager = cacheManager;
            this.signals = signals;
        }

        protected override IOrderedQueryable<Zone> MakeDefaultOrderBy(IQueryable<Zone> queryable)
        {
            return queryable.OrderBy(x => x.Name);
        }

        public Zone GetByName(string name)
        {
            var list = new List<SqlParameter>
            {
                AddInputParameter("@Name", name)
            };

            return ExecuteReaderRecord<Zone>("sp_Zones_GetByName", list.ToArray());
        }

        public override Zone GetById(int id)
        {
            return cacheManager.Get("Zones_GetById_" + id, ctx =>
            {
                ctx.Monitor(signals.When("Zones_Changed"));
                return base.GetById(id);
            });
        }

        public override void Insert(Zone record)
        {
            base.Insert(record);
            signals.Trigger("Zones_Changed");
        }

        public override void Update(Zone record)
        {
            base.Update(record);
            signals.Trigger("Zones_Changed");
        }

        public override void Delete(Zone record)
        {
            base.Delete(record);
            signals.Trigger("Zones_Changed");
        }
    }
}