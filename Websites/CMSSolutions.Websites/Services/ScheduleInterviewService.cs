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
    
    
    public interface IScheduleInterviewService : IGenericService<ScheduleInterview, int>, IDependency
    {
    }
    
    public class ScheduleInterviewService : GenericService<ScheduleInterview, int>, IScheduleInterviewService
    {
        
        public ScheduleInterviewService(IEventBus eventBus, IRepository<ScheduleInterview, int> repository) : 
                base(repository, eventBus)
        {
        }
    }
}
