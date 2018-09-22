using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.Core.Logging;

namespace CMSSolutions.Caching
{
    public class DefaultAsyncTokenProvider : IAsyncTokenProvider
    {
        public DefaultAsyncTokenProvider()
        {
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public IVolatileToken GetToken(Action<Action<IVolatileToken>> task)
        {
            var token = new AsyncVolativeToken(task, Logger);
            token.QueueWorkItem();
            return token;
        }

        public class AsyncVolativeToken : IVolatileToken
        {
            private readonly Action<Action<IVolatileToken>> task;
            private readonly List<IVolatileToken> taskTokens = new List<IVolatileToken>();
            private volatile Exception taskException;
            private volatile bool isTaskFinished;

            public AsyncVolativeToken(Action<Action<IVolatileToken>> task, ILogger logger)
            {
                this.task = task;
                Logger = logger;
            }

            public ILogger Logger { get; set; }

            public void QueueWorkItem()
            {
                // Start a work item to collect tokens in our internal array
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        task(token => taskTokens.Add(token));
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorFormat(e, "Error while monitoring extension files. Assuming extensions are not current.");
                        taskException = e;
                    }
                    finally
                    {
                        isTaskFinished = true;
                    }
                });
            }

            public bool IsCurrent
            {
                get
                {
                    // We are current until the task has finished
                    if (taskException != null)
                    {
                        return false;
                    }
                    if (isTaskFinished)
                    {
                        return taskTokens.All(t => t.IsCurrent);
                    }
                    return true;
                }
            }
        }
    }
}