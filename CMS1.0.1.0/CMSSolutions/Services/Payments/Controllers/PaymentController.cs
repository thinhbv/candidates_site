using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VortexSoft.MvcCornerStone.FormBuilder;
using VortexSoft.MvcCornerStone.Plugins;
using VortexSoft.MvcCornerStone.Services.Payments.Forms;
using VortexSoft.MvcCornerStone.Services.Payments.Models;
using VortexSoft.MvcCornerStone.Web.Themes;

namespace VortexSoft.MvcCornerStone.Services.Payments.Controllers
{
    [Themed]

    //[Authorize]
    public class PaymentController : Controller
    {
        private readonly IPluginFinder pluginFinder;
        private readonly IPaymentGateway paymentGateway;

        public PaymentController(IPluginFinder pluginFinder, IPaymentGateway paymentGateway)
        {
            this.pluginFinder = pluginFinder;
            this.paymentGateway = paymentGateway;
        }

        public ActionResult CheckoutPayment(string orderId)
        {
            var order = paymentGateway.GetOrderById(orderId);
            if (order == null)
            {
                throw new ArgumentException(string.Format("The order with id '{0}' does not exist.", orderId));
            }

            var methods = pluginFinder.GetPlugins<IPaymentMethod>().ToList();

            var model = new PaymentModel { PaymentMethods = methods };

            return new PaymentFormBuilder(this, model);
        }

        [Themed(false), HttpPost]
        public ActionResult PaymentInfo(string paymentMethod)
        {
            var methods = pluginFinder.GetPlugins<IPaymentMethod>().ToList();
            var method = methods.First(x => x.PluginDescriptor.Name == paymentMethod);
            return new PaymentInfoFormBuilder(this, method);
        }

        [Authorize(Roles = FrameworkConstants.Roles.Administrators)]
        public ActionResult Methods()
        {
            return new MethodsFormBuilder(this);
        }

        private class MethodsFormBuilder : GenericFormBuilder<object>
        {
            public MethodsFormBuilder(Controller controller)
                : base(controller, null)
            {
            }

            protected override void Build()
            {
                var grid = AddGrid<PaymentMethodModel>();
                grid.AddText(x => x.FriendlyName).HasLabelText("Friendly Name");
                grid.Bind(new List<PaymentMethodModel> { new PaymentMethodModel { FriendlyName = "hic hic" } });
            }
        }
    }
}