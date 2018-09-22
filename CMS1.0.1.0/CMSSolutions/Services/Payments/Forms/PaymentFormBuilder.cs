using System.Linq;
using System.Web.Mvc;
using VortexSoft.MvcCornerStone.FormBuilder;
using VortexSoft.MvcCornerStone.Services.Payments.Models;

namespace VortexSoft.MvcCornerStone.Services.Payments.Forms
{
    internal class PaymentFormBuilder : GenericFormBuilder<PaymentModel>
    {
        public PaymentFormBuilder(Controller controller, PaymentModel model)
            : base(controller, model)
        {
        }

        protected override void Build()
        {
            var holderControl = new HtmlTagFormControl("div");

            var onSuccess = new JQueryFunction(new[] { "data" }, new JQuery("#" + holderControl.ClientId).Html(new JQueryObject("data")));
            var data = new JQueryObject(new JQueryAssignment("paymentMethod", new JQueryObject("this.value")));
            var clickEvent = new JQueryAjax(Controller.Url.Action("PaymentInfo", "Payment")).Data(data).RequestType("POST").DataType(JQueryAjax.JQueryAjaxDataType.Html).OnSuccess(onSuccess);

            AddRadioGroup(x => x.PaymentMethod)
                .HasOptions(FormModel.PaymentMethods.ToDictionary(x => x.PluginDescriptor.Name, x => x.PluginDescriptor.FriendlyName))
                .OnClick(clickEvent);

            Add(holderControl);
        }
    }
}