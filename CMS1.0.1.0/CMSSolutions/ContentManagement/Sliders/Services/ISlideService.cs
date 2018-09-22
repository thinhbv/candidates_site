using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.ContentManagement.Sliders.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.Sliders.Services
{
    public interface ISlideService : IGenericService<Slide, Guid>, IDependency
    {
        IList<Slide> GetSlides(Guid sliderId);
    }

    [Feature(Constants.Areas.Sliders)]
    public class SlideService : GenericService<Slide, Guid>, ISlideService
    {
        public SlideService(IRepository<Slide, Guid> repository, IEventBus eventBus)
            : base(repository, eventBus)
        {
        }

        protected override IOrderedQueryable<Slide> MakeDefaultOrderBy(IQueryable<Slide> queryable)
        {
            return queryable.OrderBy(x => x.Position);
        }

        public IList<Slide> GetSlides(Guid sliderId)
        {
            return Repository.Table.Where(x => x.SliderId == sliderId).OrderBy(x => x.Position).ToList();
        }
    }
}