using System;
using System.Transactions;
using Castle.Core.Logging;

namespace CMSSolutions.Data
{
    public interface ITransactionManager : IDependency
    {
        void Demand();

        void Cancel();
    }

    public class TransactionManager : ITransactionManager, IDisposable
    {
        private TransactionScope scope;
        private bool cancelled;

        public TransactionManager()
        {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        void ITransactionManager.Demand()
        {
            if (cancelled)
            {
                try
                {
                    scope.Dispose();
                }
                catch
                {
                    // swallowing the exception
                }

                scope = null;
            }

            if (scope == null)
            {
                Logger.Debug("Creating transaction on Demand");
                scope = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel.ReadCommitted
                    });
            }
        }

        void ITransactionManager.Cancel()
        {
            Logger.Debug("Transaction cancelled flag set");
            cancelled = true;
        }

        void IDisposable.Dispose()
        {
            if (scope != null)
            {
                if (!cancelled)
                {
                    Logger.Debug("Marking transaction as complete");
                    scope.Complete();
                }

                Logger.Debug("Final work for transaction being performed");
                try
                {
                    scope.Dispose();
                }
                catch
                {
                    // swallowing the exception
                }
                Logger.Debug("Transaction disposed");
            }
        }
    }
}