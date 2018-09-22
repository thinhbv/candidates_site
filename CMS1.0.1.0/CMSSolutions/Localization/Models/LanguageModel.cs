using System;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Localization.Models
{
    public class LanguageModel
    {
        [ControlHidden]
        public int Id { get; set; }

        [ControlText(Required = true, MaxLength = 255, ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 0)]
        public string Name { get; set; }

        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Culture Code", CssClass = "uniform", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 0)]
        public string CultureCode { get; set; }

        [ControlText(ReadOnly = true, LabelText = "Culture Code", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 0)]
        public string CultureCode2 { get; set; }

        [ControlNumeric(Required = true, LabelText = "Sort Order", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 1)]
        public int SortOrder { get; set; }

        [ControlChoice(ControlChoice.DropDownList, CssClass = "uniform", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 1)]
        public string Theme { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Is Block", ContainerCssClass = "col-xs-4 col-md-4", ContainerRowIndex = 2)]
        public bool IsBlocked { get; set; }

        [ControlFileUpload(EnableFineUploader = true, LabelText = "Url Flag", ShowThumbnail = true, ContainerCssClass = "col-xs-8 col-md-8", ContainerRowIndex = 2)]
        public string ImageFlag { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Is Active", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 3)]
        public bool Active { get; set; }

        public static implicit operator LanguageModel(Domain.Language entity)
        {
            if (entity == null)
            {
                return null;
            }

            return new LanguageModel
            {
                Id = entity.Id,
                Name = entity.Name,
                CultureCode = entity.CultureCode,
                Active = entity.Active,
                SortOrder = entity.SortOrder,
                Theme = entity.Theme,
                IsBlocked = entity.IsBlocked,
                ImageFlag = entity.ImageFlag
            };
        }
    }
}