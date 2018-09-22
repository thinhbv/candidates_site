using System.Web.Mvc;
using System.Web.Routing;
using VortexSoft.MvcCornerStone.FormBuilder;

namespace VortexSoft.MvcCornerStone.Services.Payments.Forms
{
    internal class PaymentInfoFormBuilder : GenericFormBuilder<IPaymentMethod>
    {
        public PaymentInfoFormBuilder(Controller controller, IPaymentMethod model)
            : base(controller, model)
        {
        }

        protected override void Build()
        {
            string actionName;
            string controllerName;
            RouteValueDictionary routeValues;
            FormModel.GetPaymentInfoRoute(out actionName, out controllerName, out routeValues);

            Add(new ChildActionFormControl(actionName, controllerName, routeValues));
        }
    }
}