using System;
using System.Linq;
using CMSSolutions.ContentManagement.Messages.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.Messages.Services
{
    public interface ISmsMessageService : IGenericService<QueuedSms, Guid>, IDependency
    {
    }

    [Feature(Constants.Areas.Messages)]
    public class SmsMessageService : GenericService<QueuedSms, Guid>, ISmsMessageService
    {
        public SmsMessageService(IRepository<QueuedSms, Guid> repository, IEventBus eventBus)
            : base(repository, eventBus)
        {
        }

        protected override IOrderedQueryable<QueuedSms> MakeDefaultOrderBy(IQueryable<QueuedSms> queryable)
        {
            return queryable.OrderByDescending(x => x.CreatedOnUtc);
        }
    }
}