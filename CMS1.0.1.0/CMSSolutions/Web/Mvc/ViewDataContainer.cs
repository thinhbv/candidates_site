using System.Web.Mvc;

namespace CMSSolutions.Web.Mvc
{
    public class ViewDataContainer : IViewDataContainer
    {
        public ViewDataContainer(ViewDataDictionary viewData)
        {
            ViewData = viewData;
        }

        #region IViewDataContainer Members

        public ViewDataDictionary ViewData { get; set; }

        #endregion IViewDataContainer Members
    }
}