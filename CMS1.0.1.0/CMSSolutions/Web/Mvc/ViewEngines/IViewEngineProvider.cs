using System.Web.Mvc;

namespace CMSSolutions.Web.Mvc.ViewEngines
{
    public interface IViewEngineProvider : ISingletonDependency
    {
        IViewEngine CreateThemeViewEngine(CreateThemeViewEngineParams parameters);

        IViewEngine CreateModulesViewEngine(CreateModulesViewEngineParams parameters);

        /// <summary>
        /// Produce a view engine configured to resolve only fully qualified {viewName} parameters
        /// </summary>
        IViewEngine CreateBareViewEngine();
    }
}