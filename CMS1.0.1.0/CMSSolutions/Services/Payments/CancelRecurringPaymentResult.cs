using System.Collections.Generic;

namespace VortexSoft.MvcCornerStone.Services.Payments
{
    /// <summary>
    /// Represents a CancelRecurringPaymentResult
    /// </summary>
    public class CancelRecurringPaymentResult
    {
        public CancelRecurringPaymentResult()
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
    }
}