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
    
    
    public interface IInterviewService : IGenericService<Interview, int>, IDependency
    {
    }
    
    public class InterviewService : GenericService<Interview, int>, IInterviewService
    {
        
        public InterviewService(IEventBus eventBus, IRepository<Interview, int> repository) : 
                base(repository, eventBus)
        {
        }
    }
}
