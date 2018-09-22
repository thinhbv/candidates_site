using System.Collections.Generic;

namespace VortexSoft.MvcCornerStone.Services.Payments
{
    /// <summary>
    /// Represents a CapturePaymentResult
    /// </summary>
    public class CapturePaymentResult
    {
        private PaymentStatus newPaymentStatus = PaymentStatus.Pending;

        public CapturePaymentResult()
        {
            Errors = new List<string>();
        }

        public IList<string> Errors { get; set; }

        public bool Success
        {
            get { return (Errors.Count == 0); }
        }

        /// <summary>
        /// Gets or sets the capture transaction identifier
        /// </summary>
        public string CaptureTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the capture transaction result
        /// </summary>
        public string CaptureTransactionResult { get; set; }

        /// <summary>
        /// Gets or sets a payment status after processing
        /// </summary>
        public PaymentStatus NewPaymentStatus
        {
            get { return newPaymentStatus; }
            set { newPaymentStatus = value; }
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }
}