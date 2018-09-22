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
    [Url(BaseUrl = "{DashboardBaseUrl}/sms-messages")]
    [Feature(Constants.Areas.Messages)]
    public class QueuedSmsController : BaseControlController<Guid, QueuedSms, QueuedSmsModel>
    {
        public QueuedSmsController(IWorkContextAccessor workContextAccessor, ISmsMessageService messageService)
            : base(workContextAccessor, messageService)
        {
        }

        protected override bool CheckPermissions()
        {
            return CheckPermission(StandardPermissions.FullAccess);
        }

        protected override string Name
        {
            get { return "SMS Message"; }
        }

        protected override bool EnableCreate
        {
            get { return false; }
        }

        protected override bool EnableEdit
        {
            get { return false; }
        }

        protected override void OnViewIndex(ControlGridFormResult<QueuedSms> controlGrid)
        {
            controlGrid.AddColumn(x => x.FromNumber).HasHeaderText("From Number");
            controlGrid.AddColumn(x => x.ToNumber).HasHeaderText("To Number");
            controlGrid.AddColumn(x => x.CreatedOnUtc).HasHeaderText("Created On Utc");
            controlGrid.AddColumn(x => x.SentOnUtc).HasHeaderText("Sent On Utc");
            controlGrid.AddColumn(x => x.SentTries).HasHeaderText(T("Sent Tries"));
        }

        protected override void OnEditing(ControlFormResult<QueuedSmsModel> controlForm)
        {
            controlForm.Title = T("View Message");
            controlForm.ShowSubmitButton = false;
            controlForm.ReadOnly = true;
            controlForm.CancelButtonText = T("Close");
        }

        protected override QueuedSmsModel ConvertToModel(QueuedSms entity)
        {
            return entity;
        }

        protected override void ConvertFromModel(QueuedSmsModel model, QueuedSms entity)
        {
            throw new NotSupportedException();
        }

        [Url("{BaseUrl}")]
        public override ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(T("Queued SMS Messages"));
            return base.Index();
        }
    }
}