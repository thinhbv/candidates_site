using System;
using System.Threading;
using System.Web;

namespace CMSSolutions.Environment
{
    public class Starter<T> where T : class
    {
        private readonly Func<HttpApplication, T> initialization;
        private readonly Action<HttpApplication, T> beginRequest;
        private readonly Action<HttpApplication, T> endRequest;
        private readonly object synLock = new object();

        private volatile T initializationResult;

        private volatile Exception error;

        private volatile Exception previousError;

        public Starter(Func<HttpApplication, T> initialization, Action<HttpApplication, T> beginRequest, Action<HttpApplication, T> endRequest)
        {
            this.initialization = initialization;
            this.beginRequest = beginRequest;
            this.endRequest = endRequest;
        }

        public void OnApplicationStart(HttpApplication application)
        {
            LaunchStartupThread(application);
        }

        public void OnBeginRequest(HttpApplication application)
        {
            if (error != null)
            {
                bool restartInitialization = false;

                lock (synLock)
                {
                    if (error != null)
                    {
                        previousError = error;
                        error = null;
                        restartInitialization = true;
                    }
                }

                if (restartInitialization)
                {
                    LaunchStartupThread(application);
                }
            }

            if (previousError != null)
            {
                throw new ApplicationException("Error during application initialization", previousError);
            }

            if (initializationResult != null)
            {
                beginRequest(application, initializationResult);
            }
        }

        public void OnEndRequest(HttpApplication application)
        {
            if (initializationResult != null)
            {
                endRequest(application, initializationResult);
            }
        }

        public void LaunchStartupThread(HttpApplication application)
        {
            WarmupHttpModule.SignalWarmupStart();

            ThreadPool.QueueUserWorkItem(
                state =>
                {
                    try
                    {
                        var result = initialization(application);
                        initializationResult = result;
                    }
                    catch (Exception e)
                    {
                        lock (synLock)
                        {
                            error = e;
                            previousError = null;
                        }
                    }
                    finally
                    {
                        WarmupHttpModule.SignalWarmupDone();
                    }
                });
        }
    }
}