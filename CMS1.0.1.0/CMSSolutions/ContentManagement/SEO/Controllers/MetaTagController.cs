using System;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.SEO.Domain;
using CMSSolutions.ContentManagement.SEO.Models;
using CMSSolutions.ContentManagement.SEO.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.SEO.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true), Authorize]
    [Feature(Constants.Areas.SEO)]
    [Url(BaseUrl = "{DashboardBaseUrl}/meta-tags")]
    public class MetaTagController : BaseControlController<Guid, MetaTag, MetaTagModel>
    {
        public MetaTagController(IWorkContextAccessor workContextAccessor, IMetaTagService metaTagService) : base(workContextAccessor, metaTagService)
        {
        }

        protected override bool EnablePaginate
        {
            get { return false; }
        }

        protected override bool CheckPermissions()
        {
            return CheckPermission(Permissions.ManageSEO);
        }

        protected override void OnViewIndex(ControlGridFormResult<MetaTag> controlGrid)
        {
            controlGrid.AddColumn(x => x.Name);
            controlGrid.AddColumn(x => x.Content);
            controlGrid.AddColumn(x => x.Charset);
            controlGrid.ActionsColumnWidth = 150;
        }

        protected override MetaTagModel ConvertToModel(MetaTag entity)
        {
            return entity;
        }

        protected override void ConvertFromModel(MetaTagModel model, MetaTag entity)
        {
            entity.Name = model.Name;
            entity.Content = model.Content;
            entity.Charset = model.Charset;
        }
    }
}
