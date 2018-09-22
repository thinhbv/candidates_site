using System;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.Messages.Services
{
    public interface IMessageTemplateService : IGenericService<Domain.MessageTemplate, Guid>, IDependency
    {
        Domain.MessageTemplate GetTemplate(string name);
    }

    [Feature(Constants.Areas.Messages)]
    public class MessageTemplateService : GenericService<Domain.MessageTemplate, Guid>, IMessageTemplateService
    {
        private readonly ICacheManager cacheManager;
        private readonly ISignals signals;

        public MessageTemplateService(IRepository<Domain.MessageTemplate, Guid> repository, IEventBus eventBus, ICacheManager cacheManager, ISignals signals)
            : base(repository, eventBus)
        {
            this.cacheManager = cacheManager;
            this.signals = signals;
        }

        protected override IOrderedQueryable<Domain.MessageTemplate> MakeDefaultOrderBy(IQueryable<Domain.MessageTemplate> queryable)
        {
            return queryable.OrderBy(x => x.Name);
        }

        public Domain.MessageTemplate GetTemplate(string name)
        {
            return cacheManager.Get("GetTemplate_ByName_" + name, ctx =>
            {
                ctx.Monitor(signals.When("Templates_Changed"));
                return Repository.Table.FirstOrDefault(x => x.Name == name);
            });
        }

        public override void Insert(Domain.MessageTemplate record)
        {
            base.Insert(record);
            signals.Trigger("Templates_Changed");
        }

        public override void Update(Domain.MessageTemplate record)
        {
            base.Update(record);
            signals.Trigger("Templates_Changed");
        }
    }
}