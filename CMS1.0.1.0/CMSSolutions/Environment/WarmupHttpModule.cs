using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace CMSSolutions.Environment
{
    public class WarmupHttpModule : IHttpModule
    {
        private HttpApplication context;
        private static readonly object synLock = new object();
        private static IList<Action> awaiting = new List<Action>();

        public void Init(HttpApplication httpApplication)
        {
            context = httpApplication;
            context.AddOnBeginRequestAsync(BeginBeginRequest, EndBeginRequest, null);
        }

        public void Dispose()
        {
        }

        private static bool InWarmup()
        {
            lock (synLock)
            {
                return awaiting != null;
            }
        }

        /// <summary>
        /// Warmup code is about to start: Any new incoming request is queued
        /// until "SignalWarmupDone" is called.
        /// </summary>
        public static void SignalWarmupStart()
        {
            lock (synLock)
            {
                if (awaiting == null)
                {
                    awaiting = new List<Action>();
                }
            }
        }

        /// <summary>
        /// Warmup code just completed: All pending requests in the "_await" queue are processed,
        /// and any new incoming request is now processed immediately.
        /// </summary>
        public static void SignalWarmupDone()
        {
            IList<Action> temp;

            lock (synLock)
            {
                temp = awaiting;
                awaiting = null;
            }

            if (temp != null)
            {
                foreach (var action in temp)
                {
                    action();
                }
            }
        }

        /// <summary>
        /// Enqueue or directly process action depending on current mode.
        /// </summary>
        private void Await(Action action)
        {
            Action temp = action;

            lock (synLock)
            {
                if (awaiting != null)
                {
                    temp = null;
                    awaiting.Add(action);
                }
            }

            if (temp != null)
            {
                temp();
            }
        }

        private IAsyncResult BeginBeginRequest(object sender, EventArgs e, AsyncCallback cb, object extradata)
        {
            // host is available, process every requests, or file is processed
            if (!InWarmup() || WarmupUtility.DoBeginRequest(context))
            {
                var asyncResult = new DoneAsyncResult(extradata);
                cb(asyncResult);
                return asyncResult;
            }
            else
            {
                // this is the "on hold" execution path
                var asyncResult = new WarmupAsyncResult(cb, extradata);
                Await(asyncResult.Completed);
                return asyncResult;
            }
        }

        private static void EndBeginRequest(IAsyncResult ar)
        {
        }

        /// <summary>
        /// AsyncResult for "on hold" request (resumes when "Completed()" is called)
        /// </summary>
        private class WarmupAsyncResult : IAsyncResult
        {
            private readonly EventWaitHandle eventWaitHandle = new AutoResetEvent(false/*initialState*/);
            private readonly AsyncCallback asyncCallback;
            private readonly object asyncState;
            private bool isCompleted;

            public WarmupAsyncResult(AsyncCallback asyncCallback, object asyncState)
            {
                this.asyncCallback = asyncCallback;
                this.asyncState = asyncState;
                isCompleted = false;
            }

            public void Completed()
            {
                isCompleted = true;
                eventWaitHandle.Set();
                asyncCallback(this);
            }

            bool IAsyncResult.CompletedSynchronously
            {
                get { return false; }
            }

            bool IAsyncResult.IsCompleted
            {
                get { return isCompleted; }
            }

            object IAsyncResult.AsyncState
            {
                get { return asyncState; }
            }

            WaitHandle IAsyncResult.AsyncWaitHandle
            {
                get { return eventWaitHandle; }
            }
        }

        /// <summary>
        /// Async result for "ok to process now" requests
        /// </summary>
        private class DoneAsyncResult : IAsyncResult
        {
            private readonly object asyncState;
            private static readonly WaitHandle waitHandle = new ManualResetEvent(true/*initialState*/);

            public DoneAsyncResult(object asyncState)
            {
                this.asyncState = asyncState;
            }

            bool IAsyncResult.CompletedSynchronously
            {
                get { return true; }
            }

            bool IAsyncResult.IsCompleted
            {
                get { return true; }
            }

            WaitHandle IAsyncResult.AsyncWaitHandle
            {
                get { return waitHandle; }
            }

            object IAsyncResult.AsyncState
            {
                get { return asyncState; }
            }
        }
    }
}