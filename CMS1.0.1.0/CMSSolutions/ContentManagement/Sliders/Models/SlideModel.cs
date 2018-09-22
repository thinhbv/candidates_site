using System;
using CMSSolutions.ContentManagement.Sliders.Domain;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Sliders.Models
{
    public class SlideModel : BaseModel<Guid>
    {
        [ControlNumeric(Required = true)]
        public int Position { get; set; }

        [ControlText(MaxLength = 255)]
        public string Title { get; set; }

        [ControlFileUpload(EnableFineUploader = true, LabelText = "Background Url", Required = true, ShowThumbnail = true)]
        public string BackgroundUrl { get; set; }

        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Slide Direction")]
        public string SlideDirection { get; set; }

        [ControlNumeric(LabelText = "Slide Delay", MinimumValue = "1000")]
        public int? SlideDelay { get; set; }

        [ControlText(MaxLength = 255, LabelText = "Transition 2D")]
        public string Transition2D { get; set; }

        [ControlText(MaxLength = 255, LabelText = "Transition 3D")]
        public string Transition3D { get; set; }

        [ControlText(MaxLength = 255, LabelText = "On Click")]
        public string OnClick { get; set; }

        public static implicit operator SlideModel(Slide entity)
        {
            return new SlideModel
            {
                Id = entity.Id,
                Position = entity.Position,
                Title = entity.Title ?? string.Empty,
                BackgroundUrl = entity.BackgroundUrl,
                SlideDirection = entity.SlideDirection,
                SlideDelay = entity.SlideDelay,
                Transition2D = entity.Transition2D,
                Transition3D = entity.Transition3D,
                OnClick = entity.OnClick
            };
        }
    }
}