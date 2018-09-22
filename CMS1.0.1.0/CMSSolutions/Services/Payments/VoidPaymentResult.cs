using System.Collections.Generic;

namespace VortexSoft.MvcCornerStone.Services.Payments
{
    /// <summary>
    /// Represents a VoidPaymentResult
    /// </summary>
    public class VoidPaymentResult
    {
        private PaymentStatus newPaymentStatus = PaymentStatus.Pending;

        public VoidPaymentResult()
        {
            Errors = new List<string>();
        }

        public IList<string> Errors { get; set; }

        public bool Success
        {
            get { return (Errors.Count == 0); }
        }

        public void AddError(string error)
        {
            Errors.Add(error);
        }

        #region Properties

        /// <summary>
        /// Gets or sets a payment status after processing
        /// </summary>
        public PaymentStatus NewPaymentStatus
        {
            get { return newPaymentStatus; }
            set { newPaymentStatus = value; }
        }

        #endregion Properties
    }
}