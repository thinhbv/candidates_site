namespace VortexSoft.MvcCornerStone.Services.Payments
{
    /// <summary>
    /// Represents a CancelRecurringPaymentResult
    /// </summary>
    public class CancelRecurringPaymentRequest
    {
        /// <summary>
        /// Gets or sets an order
        /// </summary>
        public IOrder Order { get; set; }
    }
}