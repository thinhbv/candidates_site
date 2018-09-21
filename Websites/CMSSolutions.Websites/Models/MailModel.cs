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

		[ControlText(LabelText = "CC", Type = ControlText.TextBox, Required = false, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 0)]
		public string mail_cc { get; set; }

		[ControlText(LabelText = "BCC", Type = ControlText.TextBox, Required = false, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 0)]
		public string mail_bcc { get; set; }

		[ControlText(LabelText = "Subject", Type = ControlText.TextBox, Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 1)]
		public string subject { get; set; }

		[ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Mail Template", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 2)]
		public int template { get; set; }

		[ControlText(LabelText = "Body", Required = true, Type = ControlText.RichText, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 3)]
		public string Contents { get; set; }
	}
}