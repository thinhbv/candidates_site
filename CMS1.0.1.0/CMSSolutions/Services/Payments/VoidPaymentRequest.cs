namespace VortexSoft.MvcCornerStone.Services.Payments
{
    /// <summary>
    /// Represents a VoidPaymentResult
    /// </summary>
    public class VoidPaymentRequest
    {
        /// <summary>
        /// Gets or sets an order
        /// </summary>
        public IOrder Order { get; set; }
    }
}