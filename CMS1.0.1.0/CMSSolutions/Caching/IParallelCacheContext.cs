using System;
using System.Collections.Generic;

namespace CMSSolutions.Caching
{
    /// <summary>
    /// Provides services to enable parallel tasks aware of the current cache context.
    /// </summary>
    public interface IParallelCacheContext
    {
        /// <summary>
        /// Create a task that wraps some piece of code that implictly depends on the cache context.
        /// The return task can be used in any execution thread (e.g. System.Threading.Tasks).
        /// </summary>
        ITask<T> CreateContextAwareTask<T>(Func<T> function);

        IEnumerable<TResult> RunInParallel<T, TResult>(IEnumerable<T> source, Func<T, TResult> selector);
    }

    public interface ITask<out T> : IDisposable
    {
        /// <summary>
        /// Return tokens collected during task execution. May be empty if nothing collected,
        /// or if the task was executed in the same context as the current
        /// ICacheContextAccessor.Current.
        /// </summary>
        IEnumerable<IVolatileToken> Tokens { get; }

        /// <summary>
        /// Execute task and collect eventual volatile tokens
        /// </summary>
        T Execute();

        /// <summary>
        /// Forward collected tokens to current cache context
        /// </summary>
        void Finish();
    }
}