using System.Collections.Generic;

namespace VortexSoft.MvcCornerStone.Services.Payments
{
    public interface IRecurringPayment
    {
        /// <summary>
        /// Gets or sets the recurring payment history
        /// </summary>
        ICollection<IRecurringPaymentHistory> RecurringPaymentHistory { get; set; }
    }
}