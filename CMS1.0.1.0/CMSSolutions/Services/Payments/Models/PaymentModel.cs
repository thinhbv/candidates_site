using System.Collections.Generic;

namespace VortexSoft.MvcCornerStone.Services.Payments.Models
{
    public class PaymentModel
    {
        public string PaymentMethod { get; set; }

        public IList<IPaymentMethod> PaymentMethods { get; set; }
    }
}