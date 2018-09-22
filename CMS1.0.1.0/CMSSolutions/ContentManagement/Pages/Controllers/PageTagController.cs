using System;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Pages.Domain;
using CMSSolutions.ContentManagement.Pages.Models;
using CMSSolutions.ContentManagement.Pages.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Pages.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Url(BaseUrl = "{DashboardBaseUrl}/page-tags")]
    [Feature(Constants.Areas.Pages)]
    public class PageTagController : BaseControlController<int, PageTag, PageTagModel>
    {
        public PageTagController(IWorkContextAccessor workContextAccessor, IPageTagService service)
            : base(workContextAccessor, service)
        {
        }

        protected override int DialogModalWidth
        {
            get { return 600; }
        }

        protected override bool CheckPermissions()
        {
            return CheckPermission(PagesPermissions.ManagePages);
        }

        protected override void OnViewIndex(ControlGridFormResult<PageTag> controlGrid)
        {
            controlGrid.ActionsColumnWidth = 120;
            controlGrid.AddColumn(x => x.Name);
            controlGrid.AddColumn(x => x.Content);

            controlGrid.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            controlGrid.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;

            WorkContext.Breadcrumbs.Add(T("Pages"), Url.Action("Index", "Page"));
            WorkContext.Breadcrumbs.Add(T("Page Tags"));
        }

        protected override void OnCreating(ControlFormResult<PageTagModel> controlForm)
        {
            base.OnCreating(controlForm);
            controlForm.CssClass = "form-vertical";

            controlForm.ShowBoxHeader = false;
            controlForm.FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml;
            controlForm.FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml;
        }

        protected override void OnEditing(ControlFormResult<PageTagModel> controlForm)
        {
            base.OnEditing(controlForm);
            controlForm.CssClass = "form-vertical";

            controlForm.ShowBoxHeader = false;
            controlForm.FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml;
            controlForm.FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml;
        }

        protected override PageTagModel ConvertToModel(PageTag entity)
        {
            return entity;
        }

        protected override void ConvertFromModel(PageTagModel model, PageTag entity)
        {
            entity.Name = model.Name;
            entity.Content = model.Content;
        }
    }
}
