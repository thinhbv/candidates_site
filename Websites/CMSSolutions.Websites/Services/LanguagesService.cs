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
    
    
    public interface ILanguagesService : IGenericService<Languages, int>, IDependency
    {
    }
    
    public class LanguagesService : GenericService<Languages, int>, ILanguagesService
    {
        
        public LanguagesService(IEventBus eventBus, IRepository<Languages, int> repository) : 
                base(repository, eventBus)
        {
        }
    }
}
