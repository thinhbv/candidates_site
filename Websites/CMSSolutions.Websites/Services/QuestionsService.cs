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
    
    
    public interface IQuestionsService : IGenericService<Questions, int>, IDependency
    {
    }
    
    public class QuestionsService : GenericService<Questions, int>, IQuestionsService
    {
        
        public QuestionsService(IEventBus eventBus, IRepository<Questions, int> repository) : 
                base(repository, eventBus)
        {
        }
    }
}
