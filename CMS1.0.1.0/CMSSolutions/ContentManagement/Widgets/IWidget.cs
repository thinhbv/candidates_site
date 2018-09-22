using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using CMSSolutions.Web.UI;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Widgets
{
    public interface IWidget : IDependency
    {
        int Id { get; set; }

        string Name { get; }

        string Title { get; set; }

        bool ShowTitleOnPage { get; set; }

        bool HasTitle { get; }

        int ZoneId { get; set; }

        int? PageId { get; set; }

        int Order { get; set; }

        bool Enabled { get; set; }
        
        bool Localized { get; set; }

        string DisplayCondition { get; set; }

        string CssClass { get; set; }

        bool IsMoveable { get; set; }

        string CultureCode { get; set; }

        int? RefId { get; set; }

        IEnumerable<ResourceType> GetAdditionalResources();

        void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor);

        ActionResult DisplayCallback(Controller controller, WorkContext workContext);

        string GetDisplayCallbackUrl(UrlHelper urlHelper);

        string BuildDisplay(ViewContext viewContext, IWorkContextAccessor workContextAccessor);

        ActionResult BuildEditor(Controller controller, WorkContext workContext, ControlFormResult<IWidget> form);

        ActionResult EditorCallback(Controller controller, WorkContext workContext);

        void OnSaving(WorkContext workContext);

        IWidget ShallowCopy();
    }
}