using System;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Security.Domain;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web.Security.Models
{
    public class RoleModel : BaseModel<int>
    {
        [ControlText(Required = true, MaxLength = 255)]
        public string Name { get; set; }

        public static implicit operator RoleModel(Role other)
        {
            return new RoleModel
            {
                Id = other.Id,
                Name = other.Name
            };
        }
    }
}