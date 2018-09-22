using System.Web.Mvc;

namespace CMSSolutions.Web.Mvc
{
    public class ShapeResult : ViewResult
    {
        public ShapeResult(ControllerBase controller, dynamic shape)
        {
            ViewData = controller.ViewData;
            TempData = controller.TempData;
            ViewData.Model = shape;
            ViewName = "ShapeResult/Display";
        }
    }

    public class ShapePartialResult : PartialViewResult
    {
        public ShapePartialResult(ControllerBase controller, dynamic shape)
        {
            ViewData = controller.ViewData;
            TempData = controller.TempData;
            ViewData.Model = shape;
            ViewName = "ShapeResult/DisplayPartial";
        }
    }
}
