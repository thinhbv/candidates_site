using System;
using System.Collections.Generic;
using VortexSoft.MvcCornerStone.Services.Orders;

namespace VortexSoft.MvcCornerStone.Services.Payments
{
    public interface IPaymentGateway : IDependency
    {
        string GetStoreName();

        string GetCurrencyCode();

        /// <summary>
        /// Gets a customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>A customer</returns>
        //ICustomer GetCustomerById(int customerId);

        #region Orders

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="orderId">The order identifier</param>
        /// <returns>Order</returns>
        IOrder GetOrderById(string orderId);

        void AddOrderNote(IOrder order, string note, bool displayToCustomer, DateTime createdOnUtc);

        bool CanMarkOrderAsAuthorized(IOrder order);

        void MarkAsAuthorized(IOrder order);

        bool CanMarkOrderAsPaid(IOrder order);

        void MarkOrderAsPaid(IOrder order);

        bool CanRefundOffline(IOrder order);

        void RefundOffline(IOrder order);

        bool CanVoidOffline(IOrder order);

        void VoidOffline(IOrder order);

        #endregion Orders

        #region Recurring payments

        /// <summary>
        /// Search recurring payments
        /// </summary>
        /// <param name="customerId">The customer identifier; 0 to load all records</param>
        /// <param name="initialOrderId">The initial order identifier; 0 to load all records</param>
        /// <param name="initialOrderStatus">Initial order status identifier; null to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Recurring payment collection</returns>
        IList<IRecurringPayment> SearchRecurringPayments(int customerId,
           int initialOrderId, OrderStatus? initialOrderStatus, bool showHidden = false);

        void AddRecurringPaymentHistory(IRecurringPayment recurringPayment, int orderId, DateTime createdOnUtc);

        void ProcessNextRecurringPayment(IRecurringPayment recurringPayment);

        #endregion Recurring payments
    }
}