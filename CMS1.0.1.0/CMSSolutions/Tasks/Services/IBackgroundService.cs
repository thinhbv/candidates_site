using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Castle.Core.Logging;
using CMSSolutions.Events;

namespace CMSSolutions.Tasks.Services
{
    public interface IBackgroundService : IDependency
    {
        void Sweep();
    }

    public class BackgroundService : IBackgroundService
    {
        private readonly IEnumerable<IEventHandler> tasks;

        public BackgroundService(IEnumerable<IEventHandler> tasks)
        {
            this.tasks = tasks;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Sweep()
        {
            foreach (var task in tasks.OfType<IBackgroundTask>())
            {
                using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    try
                    {
                        task.Sweep();
                        scope.Complete();
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorFormat(e, "Error while processing background task");
                    }
                }
            }
        }
    }
}