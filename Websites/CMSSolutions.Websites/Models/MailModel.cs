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

		[ControlText(LabelText = "To", Type = ControlText.TextBox, Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 0)]
		public string mail_to { get; set; }

		[ControlChoice(ControlChoice.DropDownList, AllowMultiple = true, Required = true, LabelText = "CC", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 1)]
		public string mail_cc { get; set; }

		[ControlText(LabelText = "Subject", Type = ControlText.TextBox, Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 2)]
		public string subject { get; set; }

		[ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Mail Template", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 3)]
		public int template { get; set; }

		[ControlText(LabelText = "Body", Required = true, Type = ControlText.RichText, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 4)]
		public string Contents { get; set; }
	}
}