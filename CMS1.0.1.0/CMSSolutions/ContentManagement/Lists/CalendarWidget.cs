using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using CMSSolutions.ContentManagement.Lists.Services;
using CMSSolutions.ContentManagement.Widgets;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists
{
    [Feature(Constants.Areas.Lists)]
    public class CalendarWidget : WidgetBase
    {
        public override string Name
        {
            get { return "Calendar Widget"; }
        }

        [Display(Name = "List")]
        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "List", ContainerCssClass = "col-xs-3 col-md-3", ContainerRowIndex = 3)]
        public int ListId { get; set; }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            if (ListId == 0)
            {
                return;
            }

            var workContext = workContextAccessor.GetContext(viewContext.HttpContext);
            var listService = workContext.Resolve<IListService>();
            var list = listService.GetById(ListId);

            if (list == null)
            {
                return;
            }

            var urlHelper = workContext.Resolve<UrlHelper>();

            var id = "calendar_" + Guid.NewGuid().ToString("N").ToLowerInvariant();

            if (ShowTitleOnPage)
            {
                writer.Write("<header><h3>{0}</h3></header>", Title);
            }

            writer.Write("<div id=\"{0}\"></div>", id);

            var scriptRegister = new ScriptRegister(workContext);
            scriptRegister.IncludeInline(string.Format("$(document).ready(function(){{ $('#{0}').fullCalendar({{ events: '{1}' }}); }});", id, urlHelper.Action("GetEvents", "Home", new { area = Constants.Areas.Lists, listSlug = list.Url })));
        }

        public override ActionResult BuildEditor(Controller controller, WorkContext workContext, ControlFormResult<IWidget> form)
        {
            var listService = workContext.Resolve<IListService>();
            var list = listService.GetRecords().OrderBy(x => x.Name).ToDictionary(x => x.Id, x => x.Name);
            form.RegisterExternalDataSource("ListId", list);

            return form;
        }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            yield return ResourceType.JQueryUI;
            yield return ResourceType.FullCalendar;
        }
    }
}