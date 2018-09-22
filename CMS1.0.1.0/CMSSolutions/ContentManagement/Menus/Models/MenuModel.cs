using System;
using CMSSolutions.ContentManagement.Menus.Domain;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Menus.Models
{
    public class MenuModel
    {
        [ControlHidden]
        public int Id { get; set; }

        [ControlText(Required = true)]
        public string Name { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Is Main Menu")]
        public bool IsMainMenu { get; set; }

        public static implicit operator MenuModel(Menu other)
        {
            return new MenuModel
            {
                Id = other.Id,
                Name = other.Name,
                IsMainMenu = other.IsMainMenu
            };
        }
    }
}