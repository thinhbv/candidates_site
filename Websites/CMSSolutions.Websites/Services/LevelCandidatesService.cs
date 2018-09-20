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
    
    
    public interface ILevelCandidatesService : IGenericService<LevelCandidates, int>, IDependency
    {
    }
    
    public class LevelCandidatesService : GenericService<LevelCandidates, int>, ILevelCandidatesService
    {
        
        public LevelCandidatesService(IEventBus eventBus, IRepository<LevelCandidates, int> repository) : 
                base(repository, eventBus)
        {
        }
    }
}
