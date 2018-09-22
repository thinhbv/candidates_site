using System;
using CMSSolutions.ContentManagement.Pages.Domain;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Pages.Models
{
    public class PageModel
    {
        [ControlHidden]
        public int Id { get; set; }

        [ControlText(Required = true, MaxLength = 255, ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 0)]
        public string Title { get; set; }

        [ControlSlug(MaxLength = 255, Required = true, ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 0, ControlContainerCssClass = "input-group")]
        public string Slug { get; set; }

        [ControlText(MaxLength = 255, LabelText = "Meta Keywords", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 1)]
        public string MetaKeywords { get; set; }

        [ControlText(MaxLength = 255, LabelText = "Meta Description", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 1)]
        public string MetaDescription { get; set; }

        [ControlChoice(ControlChoice.DropDownList, LabelText = "Show On Menu", CssClass = "uniform", ContainerCssClass = "col-xs-3 col-md-3", ContainerRowIndex = 2)]
        public int? ShowOnMenuId { get; set; }

        [ControlChoice(ControlChoice.DropDownList, CssClass = "uniform", ContainerCssClass = "col-xs-3 col-md-3", ContainerRowIndex = 2)]
        public string Theme { get; set; }

        [ControlText(MaxLength = 255, LabelText = "Css Class", ContainerCssClass = "col-xs-3 col-md-3", ContainerRowIndex = 2)]
        public string CssClass { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Enabled", ContainerCssClass = "col-xs-3 col-md-3", ContainerRowIndex = 2)]
        public bool IsEnabled { get; set; }

        [ControlText(Type = ControlText.RichText, LabelText = "Body Content", HasLabelControl = false, ContainerCssClass = "col-xs-12 col-md-12", ContainerRowIndex = 3)]
        public string BodyContent { get; set; }

        [ControlHidden]
        public string CultureCode { get; set; }

        [ControlHidden]
        public int? RefId { get; set; }

        public static implicit operator PageModel(Page other)
        {
            if (other == null)
            {
                return null;
            }

            return new PageModel
            {
                Id = other.Id,
                Title = other.Title,
                Slug = other.Slug,
                MetaKeywords = other.MetaKeywords,
                MetaDescription = other.MetaDescription,
                IsEnabled = other.IsEnabled,
                BodyContent = other.BodyContent,
                CultureCode = other.CultureCode,
                RefId = other.RefId,
                Theme = other.Theme,
                CssClass = other.CssClass
            };
        }
    }
}