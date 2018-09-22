using System;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web.Security.Models
{
    public class UpdatePermissionsModel
    {
        [ControlHidden]
        public int RoleId { get; set; }

        [ControlChoice(ControlChoice.CheckBoxList, GroupedByCategory = true, LabelText = "")]
        public int[] Permissions { get; set; }
    }
}