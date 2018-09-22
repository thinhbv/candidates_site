using System.Collections.Generic;
using System.Linq;
using CMSSolutions.ContentManagement.Pages.Domain;
using CMSSolutions.ContentManagement.Widgets;
using CMSSolutions.ContentManagement.Widgets.RuleEngine;
using CMSSolutions.ContentManagement.Widgets.Services;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Pages.Widgets
{
    [Feature(Constants.Areas.Pages)]
    public class PageWidgetProvider : DefaultWidgetProvider
    {
        private readonly IWidgetService widgetService;

        public PageWidgetProvider(IWidgetService widgetService, IRuleManager ruleManager)
            : base(widgetService, ruleManager)
        {
            this.widgetService = widgetService;
        }

        public override IEnumerable<IWidget> GetWidgets(WorkContext workContext)
        {
            var currentPage = workContext.GetState<Page>("CurrentPage");

            if (currentPage == null)
            {
                return Enumerable.Empty<IWidget>();
            }

            var pageId = currentPage.RefId ?? currentPage.Id;
            var widgets = widgetService.GetWidgets(pageId);
            var currentCulture = workContext.CurrentCulture;

            return widgets.Where(x => IsVisibleWidget(widgets, x, currentCulture)).ToList();
        }
    }
}