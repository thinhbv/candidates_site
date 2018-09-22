using System;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web.Security.Models
{
    public class UpdateRolesModel
    {
        [ControlHidden]
        public int UserId { get; set; }

        [ControlChoice(ControlChoice.CheckBoxList)]
        public int[] Roles { get; set; }
    }
}