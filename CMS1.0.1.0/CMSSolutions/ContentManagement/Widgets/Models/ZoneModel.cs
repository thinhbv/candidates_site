using System;
using CMSSolutions.ContentManagement.Widgets.Domain;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Widgets.Models
{
    public class ZoneModel
    {
        [ControlHidden]
        public int Id { get; set; }

        [ControlText(Required = true, MaxLength = 255)]
        public string Name { get; set; }

        public static implicit operator ZoneModel(Zone zone)
        {
            return new ZoneModel
                {
                    Id = zone.Id,
                    Name = zone.Name
                };
        }
    }
}