namespace CMSSolutions.Websites.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CMSSolutions;
    using CMSSolutions.Caching;
    using CMSSolutions.Websites.Entities;
    using CMSSolutions.Events;
    using CMSSolutions.Services;
    using CMSSolutions.Data;
    
    
    public interface IMailTemplatesService : IGenericService<MailTemplates, int>, IDependency
    {
    }
    
    public class MailTemplatesService : GenericService<MailTemplates, int>, IMailTemplatesService
    {
        
        public MailTemplatesService(IEventBus eventBus, IRepository<MailTemplates, int> repository) : 
                base(repository, eventBus)
        {
        }
    }
}
