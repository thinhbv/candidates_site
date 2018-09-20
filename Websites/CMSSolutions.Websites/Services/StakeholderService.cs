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
    
    
    public interface IStakeholderService : IGenericService<Stakeholder, int>, IDependency
    {
    }
    
    public class StakeholderService : GenericService<Stakeholder, int>, IStakeholderService
    {
        
        public StakeholderService(IEventBus eventBus, IRepository<Stakeholder, int> repository) : 
                base(repository, eventBus)
        {
        }
    }
}
