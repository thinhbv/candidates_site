using System;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Sliders.Domain;
using CMSSolutions.ContentManagement.Sliders.Models;
using CMSSolutions.ContentManagement.Sliders.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Sliders.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Url(BaseUrl = "{DashboardBaseUrl}/slideshows")]
    [Feature(Constants.Areas.Sliders)]
    public class SliderController : BaseControlController<Guid, Slider, SliderModel>
    {
        public SliderController(IWorkContextAccessor workContextAccessor, ISliderService service)
            : base(workContextAccessor, service)
        {
        }

        protected override string Name
        {
            get { return "Slideshow"; }
        }

        protected override bool CheckPermissions()
        {
            return CheckPermission(StandardPermissions.FullAccess);
        }

        protected override void OnViewIndex(ControlGridFormResult<Slider> controlGrid)
        {
            controlGrid.ActionsColumnWidth = 200;
            controlGrid.AddColumn(x => x.Name);
            controlGrid.AddColumn(x => x.Width);
            controlGrid.AddColumn(x => x.Height);

            controlGrid.AddRowAction()
                .HasText(T("Slides"))
                .HasUrl(x => Url.Action("Index", "Slide", new { sliderId = x.Id }))
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Primary);
        }

        protected override SliderModel ConvertToModel(Slider entity)
        {
            return entity;
        }

        protected override void ConvertFromModel(SliderModel model, Slider entity)
        {
            entity.Id = model.Id;
            entity.Name = model.Name;
            entity.Width = model.Width;
            entity.Height = model.Height;
        }

        [Url("{BaseUrl}")]
        public override ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(T("Sliders"));
            return base.Index();
        }
    }
}