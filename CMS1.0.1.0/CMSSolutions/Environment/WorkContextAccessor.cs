using System;
using System.Collections.Concurrent;
using System.Web;
using Autofac;

namespace CMSSolutions.Environment
{
    public class WorkContextAccessor : IWorkContextAccessor
    {
        private readonly ILifetimeScope lifetimeScope;

        private readonly IHttpContextAccessor httpContextAccessor;

        // a different symbolic key is used for each tenant.
        // this guarantees the correct accessor is being resolved.
        private readonly object workContextKey = new object();

        [ThreadStatic]
        private static ConcurrentDictionary<object, WorkContext> threadStaticContexts;

        public WorkContextAccessor(
            IHttpContextAccessor httpContextAccessor,
            ILifetimeScope lifetimeScope)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.lifetimeScope = lifetimeScope;
        }

        public WorkContext GetContext(HttpContextBase httpContext)
        {
            return httpContext.Items[workContextKey] as WorkContext;
        }

        public WorkContext GetContext()
        {
            var httpContext = httpContextAccessor.Current();
            if (httpContext != null)
                return GetContext(httpContext);

            WorkContext workContext;
            return EnsureThreadStaticContexts().TryGetValue(workContextKey, out workContext) ? workContext : null;
        }

        public IWorkContextScope CreateWorkContextScope(HttpContextBase httpContext)
        {
            var workLifetime = lifetimeScope.BeginLifetimeScope("work");
            workLifetime.Resolve<WorkContextProperty<HttpContextBase>>().Value = httpContext;

            return new HttpContextScopeImplementation(
                workLifetime,
                httpContext,
                workContextKey);
        }

        public IWorkContextScope CreateWorkContextScope()
        {
            var httpContext = httpContextAccessor.Current();
            if (httpContext != null)
                return CreateWorkContextScope(httpContext);

            return new ThreadStaticScopeImplementation(
                lifetimeScope.BeginLifetimeScope("work"),
                EnsureThreadStaticContexts(),
                workContextKey);
        }

        private static ConcurrentDictionary<object, WorkContext> EnsureThreadStaticContexts()
        {
            return threadStaticContexts ?? (threadStaticContexts = new ConcurrentDictionary<object, WorkContext>());
        }

        private class HttpContextScopeImplementation : IWorkContextScope
        {
            private readonly WorkContext workContext;
            private readonly Action disposer;

            public HttpContextScopeImplementation(ILifetimeScope lifetimeScope, HttpContextBase httpContext, object workContextKey)
            {
                workContext = lifetimeScope.Resolve<WorkContext>();
                httpContext.Items[workContextKey] = workContext;
                disposer = () =>
                {
                    httpContext.Items.Remove(workContextKey);
                    lifetimeScope.Dispose();
                };
            }

            void IDisposable.Dispose()
            {
                disposer();
            }

            public WorkContext WorkContext
            {
                get { return workContext; }
            }

            public TService Resolve<TService>()
            {
                return WorkContext.Resolve<TService>();
            }

            public bool TryResolve<TService>(out TService service)
            {
                return WorkContext.TryResolve(out service);
            }
        }

        private class ThreadStaticScopeImplementation : IWorkContextScope
        {
            private readonly WorkContext workContext;
            private readonly Action disposer;

            public ThreadStaticScopeImplementation(ILifetimeScope lifetimeScope, ConcurrentDictionary<object, WorkContext> contexts, object workContextKey)
            {
                workContext = lifetimeScope.Resolve<WorkContext>();
                contexts.AddOrUpdate(workContextKey, workContext, (a, b) => workContext);
                disposer = () =>
                {
                    WorkContext removedContext;
                    contexts.TryRemove(workContextKey, out removedContext);
                    lifetimeScope.Dispose();
                };
            }

            void IDisposable.Dispose()
            {
                disposer();
            }

            public WorkContext WorkContext
            {
                get { return workContext; }
            }

            public TService Resolve<TService>()
            {
                return WorkContext.Resolve<TService>();
            }

            public bool TryResolve<TService>(out TService service)
            {
                return WorkContext.TryResolve(out service);
            }
        }
    }
}