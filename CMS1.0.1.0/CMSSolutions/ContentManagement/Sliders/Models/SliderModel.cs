using System;
using CMSSolutions.ContentManagement.Sliders.Domain;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Sliders.Models
{
    public class SliderModel : BaseModel<Guid>
    {
        [ControlText(Required = true, MaxLength = 255)]
        public string Name { get; set; }

        [ControlNumeric(Required = true)]
        public int Width { get; set; }

        [ControlNumeric(Required = true)]
        public int Height { get; set; }

        public static implicit operator SliderModel(Slider entity)
        {
            return new SliderModel
                   {
                       Id = entity.Id,
                       Name = entity.Name,
                       Width = entity.Width,
                       Height = entity.Height
                   };
        }
    }
}