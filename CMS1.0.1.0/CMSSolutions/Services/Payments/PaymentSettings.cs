using System;
using System.Collections.Generic;
using VortexSoft.MvcCornerStone.Configuration;

namespace VortexSoft.MvcCornerStone.Services.Payments
{
    [Serializable]
    public class PaymentSettings : ISettings
    {
        public PaymentSettings()
        {
            ActivePaymentMethodNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets a system names of active payment methods
        /// </summary>
        public List<string> ActivePaymentMethodNames { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users are allowed to repost (complete) payments for redirection payment methods
        /// </summary>
        public bool AllowRePostingPayments { get; set; }
    }
}