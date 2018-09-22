using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.ContentManagement.SEO.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.SEO.Services
{
    public interface IMetaTagService : IGenericService<MetaTag, Guid>, IDependency
    {
        
    }

    [Feature(Constants.Areas.SEO)]
    public class MetaTagService : GenericService<MetaTag, Guid>, IMetaTagService
    {
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;

        public MetaTagService(IRepository<MetaTag, Guid> repository, IEventBus eventBus, ICacheManager cacheManager, ISignals signals)
            : base(repository, eventBus)
        {
            this.cacheManager = cacheManager;
            this.signals = signals;
        }

        protected override IOrderedQueryable<MetaTag> MakeDefaultOrderBy(IQueryable<MetaTag> queryable)
        {
            return queryable.OrderBy(x => x.Name);
        }

        public override void Insert(MetaTag record)
        {
            base.Insert(record);
            signals.Trigger("MetaTags_Changed");
        }

        public override void Update(MetaTag record)
        {
            base.Update(record);
            signals.Trigger("MetaTags_Changed");
        }

        public override void Delete(MetaTag record)
        {
            base.Delete(record);
            signals.Trigger("MetaTags_Changed");
        }

        public override IList<MetaTag> GetRecords()
        {
            return cacheManager.Get("MetaTags_GetMetaTags", ctx =>
            {
                ctx.Monitor(signals.When("MetaTags_Changed"));

                return base.GetRecords();
            });
        }
    }
}
