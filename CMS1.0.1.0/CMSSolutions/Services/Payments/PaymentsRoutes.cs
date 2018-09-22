using System;
using System.Collections.Generic;
using VortexSoft.MvcCornerStone.Configuration;
using VortexSoft.MvcCornerStone.Web.Mvc.Routes;

namespace VortexSoft.MvcCornerStone.Services.Payments
{
    public class PaymentsRoutes : RouteProviderBase, IRouteProvider
    {
        protected override int Priority
        {
            get { return 10; }
        }

        #region IRouteProvider Members

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            var routes = new List<RouteDescriptor>();

            if (!FrameworkConfigurationSection.Instance.EnablePayments)
            {
                return routes;
            }

            var config = FrameworkConfigurationSection.Instance.Payments;

            if (!config.CheckoutPaymentUrl.Contains("{orderId}"))
            {
                throw new ArgumentException("The config path framework/payments/checkoutPaymentUrl does not correct.");
            }

            routes.Add(GetRouteDescriptor("Payments-CheckoutPayment", config.CheckoutPaymentUrl,
                                          new { controller = "Payment", action = "CheckoutPayment" },
                                          new[] { "VortexSoft.MvcCornerStone.Services.Payments.Controllers" }));

            routes.Add(GetRouteDescriptor("Payments-Methods", config.PaymentMethodsUrl,
                                          new { controller = "Payment", action = "Methods" },
                                          new[] { "VortexSoft.MvcCornerStone.Services.Payments.Controllers" }));

            routes.Add(GetRouteDescriptor("Payments-Actions", config.PaymentActionsUrl,
                                          new { controller = "Payment" },
                                          new[] { "VortexSoft.MvcCornerStone.Services.Payments.Controllers" }));

            return routes;
        }

        #endregion IRouteProvider Members
    }
}