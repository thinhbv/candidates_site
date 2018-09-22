using System;
using System.IO;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Aliases.Domain;
using CMSSolutions.ContentManagement.Aliases.Models;
using CMSSolutions.ContentManagement.Aliases.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Aliases.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Aliases)]
    [Url(BaseUrl = "{DashboardBaseUrl}/alias")]
    public class AliasController : BaseControlController<int, Alias, AliasModel>
    {
        public AliasController(IWorkContextAccessor workContextAccessor, IAliasService aliasService)
            : base(workContextAccessor, aliasService)
        {
           
        }

        protected override bool CheckPermissions()
        {
            return CheckPermission(StandardPermissions.FullAccess);
        }

        protected override void OnViewIndex(ControlGridFormResult<Alias> controlGrid)
        {
            controlGrid.AddColumn(x => x.Path).HasHeaderText("Alias");
            controlGrid.AddColumn(x => x.RouteValues).HasHeaderText("Route");
            controlGrid.AddColumn(x => x.IsEnabled).RenderAsStatusImage().HasHeaderText("Enabled");
        }

        [ValidateInput(false), FormButton("Save")]
        [HttpPost, Url("{BaseUrl}/update")]
        public override ActionResult Update(AliasModel model)
        {
            return base.Update(model);
        }

        protected override AliasModel ConvertToModel(Alias entity)
        {
            return entity;
        }

        protected override void ConvertFromModel(AliasModel model, Alias entity)
        {
            entity.Id = model.Id;
            entity.Path = model.Path;
            entity.Source = model.Source;
            entity.IsEnabled = model.IsEnabled;
        }

        [Url("{BaseUrl}")]
        public override ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(T("Aliases"));
            return base.Index();
        }
    }
}