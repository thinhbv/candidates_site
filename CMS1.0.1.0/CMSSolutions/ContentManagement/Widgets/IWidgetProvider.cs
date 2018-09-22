using System.Collections.Generic;
using System.Linq;
using CMSSolutions.ContentManagement.Widgets.RuleEngine;
using CMSSolutions.ContentManagement.Widgets.Services;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Widgets
{
    public interface IWidgetProvider : IDependency
    {
        IEnumerable<IWidget> GetWidgets(WorkContext workContext);
    }

    [Feature(Constants.Areas.Widgets)]
    public class DefaultWidgetProvider : IWidgetProvider
    {
        private readonly IWidgetService widgetService;
        private readonly IRuleManager ruleManager;

        public DefaultWidgetProvider(IWidgetService widgetService, IRuleManager ruleManager)
        {
            this.widgetService = widgetService;
            this.ruleManager = ruleManager;
        }

        public virtual IEnumerable<IWidget> GetWidgets(WorkContext workContext)
        {
            var currentCulture = workContext.CurrentCulture;
            var widgets = widgetService.GetWidgets();
            return widgets.Where(x => IsVisibleWidget(widgets, x, currentCulture)).ToList();
        }

        protected bool IsVisibleWidget(IEnumerable<IWidget> widgets, IWidget widget, string currentCulture)
        {
            if (widget.RefId.HasValue)
            {
                if (!string.Equals(widget.CultureCode, currentCulture))
                {
                    return false;
                }

                var parentWidget = widgets.FirstOrDefault(x => x.Id == widget.RefId.Value);
                if (parentWidget == null || !parentWidget.Enabled)
                {
                    return false;
                }
            }
            else
            {
                var childWidget = widgets.FirstOrDefault(x => x.RefId == widget.Id && x.CultureCode == currentCulture);
                if (childWidget != null)
                {
                    return false;
                }
            }

            return string.IsNullOrEmpty(widget.DisplayCondition) || ruleManager.Matches(widget.DisplayCondition);
        }
    }
}