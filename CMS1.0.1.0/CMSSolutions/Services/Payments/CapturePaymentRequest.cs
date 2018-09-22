namespace VortexSoft.MvcCornerStone.Services.Payments
{
    /// <summary>
    /// Represents a CapturePaymentRequest
    /// </summary>
    public class CapturePaymentRequest
    {
        /// <summary>
        /// Gets or sets an order
        /// </summary>
        public IOrder Order { get; set; }
    }
}