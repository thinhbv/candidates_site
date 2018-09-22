using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Castle.Core.Logging;
using CMSSolutions.DisplayManagement;
using CMSSolutions.Web.Mvc.Spooling;
using CMSSolutions.Web.Themes;

namespace CMSSolutions.Web.Mvc.ViewEngines.ThemeAwareness
{
    public interface ILayoutAwareViewEngine : IDependency, IViewEngine
    {
    }

    public class LayoutAwareViewEngine : ILayoutAwareViewEngine
    {
        private readonly WorkContext workContext;
        private readonly IThemeAwareViewEngine themeAwareViewEngine;
        private readonly IDisplayHelperFactory displayHelperFactory;

        public LayoutAwareViewEngine(
            WorkContext workContext,
            IThemeAwareViewEngine themeAwareViewEngine,
            IDisplayHelperFactory displayHelperFactory)
        {
            this.workContext = workContext;
            this.themeAwareViewEngine = themeAwareViewEngine;
            this.displayHelperFactory = displayHelperFactory;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return themeAwareViewEngine.FindPartialView(controllerContext, partialViewName, useCache, true);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            var viewResult = themeAwareViewEngine.FindPartialView(controllerContext, viewName, useCache, true);

            if (viewResult.View == null)
            {
                return viewResult;
            }

            ThemedAttribute attribute;
            if (!ThemeFilter.IsApplied(controllerContext.RequestContext, out attribute))
            {
                return viewResult;
            }

            var layoutView = new LayoutView((viewContext, writer, viewDataContainer) =>
            {
                Logger.Info("Rendering layout view");

                var childContentWriter = new HtmlStringWriter();

                var childContentViewContext = new ViewContext(
                    viewContext,
                    viewContext.View,
                    viewContext.ViewData,
                    viewContext.TempData,
                    childContentWriter);

                viewResult.View.Render(childContentViewContext, childContentWriter);
                workContext.Layout.Metadata.ChildContent = childContentWriter;

                var display = displayHelperFactory.CreateHelper(viewContext, viewDataContainer);
                IHtmlString result = display(workContext.Layout);
                writer.Write(result.ToHtmlString());

                Logger.Info("Done rendering layout view");
            }, (context, view) => viewResult.ViewEngine.ReleaseView(context, viewResult.View));

            return new ViewEngineResult(layoutView, this);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            var layoutView = (LayoutView)view;
            layoutView.ReleaseView(controllerContext, view);
        }

        private class LayoutView : IView, IViewDataContainer
        {
            private readonly Action<ViewContext, TextWriter, IViewDataContainer> render;
            private readonly Action<ControllerContext, IView> releaseView;

            public LayoutView(Action<ViewContext, TextWriter, IViewDataContainer> render, Action<ControllerContext, IView> releaseView)
            {
                this.render = render;
                this.releaseView = releaseView;
            }

            public ViewDataDictionary ViewData { get; set; }

            public void Render(ViewContext viewContext, TextWriter writer)
            {
                ViewData = viewContext.ViewData;
                render(viewContext, writer, this);
            }

            public void ReleaseView(ControllerContext context, IView view)
            {
                releaseView(context, view);
            }
        }
    }
}