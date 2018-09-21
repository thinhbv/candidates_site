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
    
    
    public interface ILevelsService : IGenericService<Levels, int>, IDependency
    {
    }
    
    public class LevelsService : GenericService<Levels, int>, ILevelsService
    {
        
        public LevelsService(IEventBus eventBus, IRepository<Levels, int> repository) : 
                base(repository, eventBus)
        {
        }
    }
}
