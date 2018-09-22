namespace VortexSoft.MvcCornerStone.Services.Payments
{
    public interface IOrder
    {
        int Id { get; set; }

        PaymentStatus PaymentStatus { get; set; }

        string PaymentMethodName { get; set; }

        decimal OrderTotal { get; set; }

        /// <summary>
        /// Gets or sets the authorization transaction identifier
        /// </summary>
        string AuthorizationTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the capture transaction identifier
        /// </summary>
        string CaptureTransactionId { get; set; }
    }
}