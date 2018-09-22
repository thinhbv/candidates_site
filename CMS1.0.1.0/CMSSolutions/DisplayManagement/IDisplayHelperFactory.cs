using System.Web.Mvc;

namespace CMSSolutions.DisplayManagement
{
    /// <summary>
    /// Used to create a dynamic, contextualized Display object to dispatch shape rendering
    /// </summary>
    public interface IDisplayHelperFactory : IDependency
    {
        dynamic CreateHelper(ViewContext viewContext, IViewDataContainer viewDataContainer);
    }
}