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
    
    
    public interface ICandidatesService : IGenericService<Candidates, int>, IDependency
    {
    }
    
    public class CandidatesService : GenericService<Candidates, int>, ICandidatesService
    {
        
        public CandidatesService(IEventBus eventBus, IRepository<Candidates, int> repository) : 
                base(repository, eventBus)
        {
        }
    }
}
