using System;
using System.Collections.Generic;
using System.Linq;

namespace CMSSolutions.Caching
{
    public class DefaultParallelCacheContext : IParallelCacheContext
    {
        private readonly ICacheContextAccessor cacheContextAccessor;

        public DefaultParallelCacheContext(ICacheContextAccessor cacheContextAccessor)
        {
            this.cacheContextAccessor = cacheContextAccessor;
        }

        /// <summary>
        ///  Allow disabling parallel behavior through HostComponents.config
        /// </summary>
        public bool Disabled { get; set; }

        #region IParallelCacheContext Members

        public IEnumerable<TResult> RunInParallel<T, TResult>(IEnumerable<T> source, Func<T, TResult> selector)
        {
            if (Disabled)
            {
                return source.Select(selector);
            }

            // Create tasks that capture the current thread context
            List<ITask<TResult>> tasks =
                source.Select(item => CreateContextAwareTask(() => selector(item))).ToList();

            // Run tasks in parallel and combine results immediately
            TResult[] result = tasks
                .AsParallel() // prepare for parallel execution
                .AsOrdered() // preserve initial enumeration order
                .Select(task => task.Execute()) // prepare tasks to run in parallel
                .ToArray(); // force evaluation

            // Forward tokens collected by tasks to the current context
            foreach (var task in tasks)
            {
                task.Finish();
            }
            return result;
        }

        /// <summary>
        /// Create a task that wraps some piece of code that implictly depends on the cache context.
        /// The return task can be used in any execution thread (e.g. System.Threading.Tasks).
        /// </summary>
        public ITask<T> CreateContextAwareTask<T>(Func<T> function)
        {
            return new TaskWithAcquireContext<T>(cacheContextAccessor, function);
        }

        #endregion IParallelCacheContext Members

        #region Nested type: TaskWithAcquireContext

        public class TaskWithAcquireContext<T> : ITask<T>
        {
            private readonly ICacheContextAccessor cacheContextAccessor;
            private readonly Func<T> function;
            private IList<IVolatileToken> tokens;

            public TaskWithAcquireContext(ICacheContextAccessor cacheContextAccessor, Func<T> function)
            {
                this.cacheContextAccessor = cacheContextAccessor;
                this.function = function;
            }

            #region ITask<T> Members

            /// <summary>
            /// Execute task and collect eventual volatile tokens
            /// </summary>
            public T Execute()
            {
                IAcquireContext parentContext = cacheContextAccessor.Current;
                try
                {
                    // Push context
                    if (parentContext == null)
                    {
                        cacheContextAccessor.Current = new SimpleAcquireContext(AddToken);
                    }

                    // Execute lambda
                    return function();
                }
                finally
                {
                    // Pop context
                    if (parentContext == null)
                    {
                        cacheContextAccessor.Current = null;
                    }
                }
            }

            /// <summary>
            /// Return tokens collected during task execution
            /// </summary>
            public IEnumerable<IVolatileToken> Tokens
            {
                get
                {
                    if (tokens == null)
                        return Enumerable.Empty<IVolatileToken>();
                    return tokens;
                }
            }

            public void Dispose()
            {
                Finish();
            }

            /// <summary>
            /// Forward collected tokens to current cache context
            /// </summary>
            public void Finish()
            {
                IList<IVolatileToken> tokens1 = tokens;
                tokens = null;
                if (cacheContextAccessor.Current != null && tokens1 != null)
                {
                    foreach (IVolatileToken token in tokens1)
                    {
                        cacheContextAccessor.Current.Monitor(token);
                    }
                }
            }

            #endregion ITask<T> Members

            private void AddToken(IVolatileToken token)
            {
                if (tokens == null)
                    tokens = new List<IVolatileToken>();
                tokens.Add(token);
            }
        }

        #endregion Nested type: TaskWithAcquireContext
    }
}