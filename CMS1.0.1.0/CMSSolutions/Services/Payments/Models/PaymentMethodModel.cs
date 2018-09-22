namespace VortexSoft.MvcCornerStone.Services.Payments.Models
{
    public class PaymentMethodModel
    {
        public virtual string FriendlyName { get; set; }

        public string Name { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public bool SupportCapture { get; set; }

        public bool SupportPartiallyRefund { get; set; }

        public bool SupportRefund { get; set; }

        public bool SupportVoid { get; set; }

        public RecurringPaymentType RecurringPaymentType { get; set; }
    }
}