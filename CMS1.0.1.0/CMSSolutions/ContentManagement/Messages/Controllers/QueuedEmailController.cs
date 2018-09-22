using System;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Messages.Domain;
using CMSSolutions.ContentManagement.Messages.Models;
using CMSSolutions.ContentManagement.Messages.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Messages.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Url(BaseUrl = "{DashboardBaseUrl}/messages")]
    [Feature(Constants.Areas.Messages)]
    public class QueuedEmailController : BaseControlController<Guid, QueuedEmail, QueuedEmailModel>
    {
        public QueuedEmailController(IWorkContextAccessor workContextAccessor, IMessageService messageService)
            : base(workContextAccessor, messageService)
        {
        }

        protected override bool CheckPermissions()
        {
            return CheckPermission(StandardPermissions.FullAccess);
        }

        protected override string Name
        {
            get { return "Email Message"; }
        }

        protected override bool EnableCreate
        {
            get { return false; }
        }

        protected override bool EnableEdit
        {
            get { return false; }
        }

        protected override void OnViewIndex(ControlGridFormResult<QueuedEmail> controlGrid)
        {
            controlGrid.AddColumn(x => x.Subject);
            controlGrid.AddColumn(x => x.ToAddress).HasHeaderText("To Address");
            controlGrid.AddColumn(x => x.CreatedOnUtc).HasHeaderText("Created On Utc");
            controlGrid.AddColumn(x => x.SentOnUtc).HasHeaderText("Sent On Utc");
            controlGrid.AddColumn(x => x.SentTries).HasHeaderText(T("Sent Tries"));

            controlGrid.AddAction()
                .HasText(T("SMTP Settings"))
                .HasUrl(Url.Action("Edit", "Settings", new
                {
                    area = Constants.Areas.Core,
                    id = "CMSSolutions.Net.Mail.SmtpSettings",
                    returnUrl = Url.Action("Index")
                }))
                .HasIconCssClass("cx-icon cx-icon-mail")
                .HasButtonStyle(ButtonStyle.Success).HasBoxButton(false)
                .HasRow(false)
                .HasCssClass(Constants.RowLeft);
        }

        protected override void OnEditing(ControlFormResult<QueuedEmailModel> controlForm)
        {
            controlForm.Title = T("View Message");
            controlForm.ShowSubmitButton = false;
            controlForm.ReadOnly = true;
            controlForm.CancelButtonText = T("Close");
        }

        protected override QueuedEmailModel ConvertToModel(QueuedEmail entity)
        {
            return entity;
        }

        protected override void ConvertFromModel(QueuedEmailModel model, QueuedEmail entity)
        {
            throw new NotSupportedException();
        }

        [Url("{BaseUrl}")]
        public override ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(T("Queued Emails"));
            return base.Index();
        }
    }
}