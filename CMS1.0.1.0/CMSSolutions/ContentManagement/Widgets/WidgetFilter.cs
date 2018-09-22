using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Widgets.Services;
using CMSSolutions.DisplayManagement;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc.Filters;
using CMSSolutions.Web.UI;

namespace CMSSolutions.ContentManagement.Widgets
{
    [Feature(Constants.Areas.Widgets)]
    public class WidgetFilter : FilterProvider, IResultFilter
    {
        private readonly IWorkContextAccessor workContextAccessor;
        private readonly dynamic shapeFactory;
        private readonly IEnumerable<IWidgetProvider> providers;
        private readonly IZoneService zoneService;

        public WidgetFilter(IWorkContextAccessor workContextAccessor, IShapeFactory shapeFactory, IEnumerable<IWidgetProvider> providers, IZoneService zoneService)
        {
            this.shapeFactory = shapeFactory;
            this.providers = providers;
            this.zoneService = zoneService;
            this.workContextAccessor = workContextAccessor;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult))
            {
                return;
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                return;
            }

            var workContext = workContextAccessor.GetContext(filterContext);

            if (workContext == null || workContext.Layout == null)
            {
                return;
            }

            var widgets = providers.SelectMany(x => x.GetWidgets(workContext)).ToList();
            var zones = workContext.Layout.Zones;

            var resourceTypes = new List<ResourceType>();

            foreach (var widget in widgets)
            {
                resourceTypes.AddRange(widget.GetAdditionalResources());

                var widgetShape = shapeFactory.Widget();
                widgetShape.Widget = widget;

                var zoneRecord = zoneService.GetById(widget.ZoneId);
                if (zoneRecord != null)
                {
                    var zone = zones[zoneRecord.Name];
                    zone.Add(widgetShape, widget.Order.ToString(CultureInfo.InvariantCulture));
                }
            }

            if (resourceTypes.Count > 0)
            {
                var scriptRegister = new ScriptRegister(workContext);
                var styleRegister = new StyleRegister(workContext);

                foreach (var resourceType in resourceTypes)
                {
                    ResourcesManager.LookupResources(resourceType, scriptRegister, styleRegister);
                }
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}