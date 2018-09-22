using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
    [Feature(Constants.Areas.Messages)]
    [Url(BaseUrl = "{DashboardBaseUrl}/message-templates")]
    public class MessageTemplateController : BaseControlController<Guid, Domain.MessageTemplate, MessageTemplateModel>
    {
        private readonly IEnumerable<IMessageTokensProvider> tokenProviders;

        public MessageTemplateController(
            IWorkContextAccessor workContextAccessor,
            IMessageTemplateService messageTemplateService,
            IEnumerable<IMessageTokensProvider> tokenProviders)
            : base(workContextAccessor, messageTemplateService)
        {
            this.tokenProviders = tokenProviders;
        }

        protected override bool CheckPermissions()
        {
            return CheckPermission(StandardPermissions.FullAccess);
        }

        protected override int DialogModalWidth
        {
            get { return 800; }
        }

        protected override bool EnableCreate
        {
            get { return false; }
        }

        protected override bool EnableDelete
        {
            get { return false; }
        }

        protected override void OnViewIndex(ControlGridFormResult<Domain.MessageTemplate> controlGrid)
        {
            controlGrid.AddColumn(x => x.Name);
            controlGrid.AddColumn(x => x.Subject);
            controlGrid.AddColumn(x => x.Enabled).RenderAsStatusImage();
        }

        protected override void OnEditing(ControlFormResult<MessageTemplateModel> controlForm)
        {
            var allTokens = new List<string>();

            foreach (var tokens in tokenProviders.Select(provider => provider.GetAvailableTokens(controlForm.FormModel.Name)).Where(tokens => tokens != null))
            {
                allTokens.AddRange(tokens);
            }

            controlForm.FormModel.Tokens = string.Join(" ", allTokens.Distinct().OrderBy(x => x));
        }

        protected override MessageTemplateModel ConvertToModel(Domain.MessageTemplate entity)
        {
            return entity;
        }

        protected override void ConvertFromModel(MessageTemplateModel model, Domain.MessageTemplate entity)
        {
            entity.Subject = model.Subject;
            entity.Name = model.Name;
            entity.Body = model.Body;
            entity.Enabled = model.Enabled;
        }

        [Url("{BaseUrl}")]
        public override ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(T("Messages"));
            return base.Index();
        }
    }
}