using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Websites.Models
{
	public class MailModel
	{
		[ControlHidden()]
		public int candidate_id { get; set; }

		[ControlText(LabelText = "Mail To", Type = ControlText.TextBox, Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol4, ContainerRowIndex = 1)]
		public string mail_to { get; set; }

		[ControlChoice(ControlChoice.DropDownList, LabelText = "Mail CC", AllowMultiple = true, CssClass = Extensions.Constants.CssControlCustom, EnableChosen = true, Required = true, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 2)]
		public int[] mail_cc { get; set; }

		[ControlText(LabelText = "Subject", Type = ControlText.TextBox, Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 3)]
		public string subject { get; set; }

		[ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Mail Template", ContainerCssClass = Constants.ContainerCssClassCol4, ContainerRowIndex = 4, OnSelectedIndexChanged = "$('#" + Extensions.Constants.MailBody + "').empty();")]
		public int template { get; set; }

		[ControlCascadingDropDown(LabelText = "Recruitment Information", ParentControl = "template", AbsoluteParentControl = true, ContainerCssClass = Constants.ContainerCssClassCol4, ContainerRowIndex = 5)]
		public int mail_body { get; set; }
	}
}