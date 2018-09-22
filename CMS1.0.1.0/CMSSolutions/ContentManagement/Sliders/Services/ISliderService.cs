using System;
using CMSSolutions.ContentManagement.Sliders.Domain;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Services;

namespace CMSSolutions.ContentManagement.Sliders.Services
{
    public interface ISliderService : IGenericService<Slider, Guid>, IDependency
    {
    }

    [Feature(Constants.Areas.Sliders)]
    public class SliderService : GenericService<Slider, Guid>, ISliderService
    {
        public SliderService(IRepository<Slider, Guid> repository, IEventBus eventBus)
            : base(repository, eventBus)
        {
        }
    }
}