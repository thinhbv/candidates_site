using System;
using System.Timers;
using Castle.Core.Logging;
using CMSSolutions.Data;
using CMSSolutions.Tasks.Services;

namespace CMSSolutions.Tasks
{
    public interface ISweepGenerator : ISingletonDependency
    {
        void Activate();

        void Terminate();
    }

    public class SweepGenerator : ISweepGenerator, IDisposable
    {
        private readonly IWorkContextAccessor workContextAccessor;
        private readonly Timer timer;

        public SweepGenerator(IWorkContextAccessor workContextAccessor)
        {
            this.workContextAccessor = workContextAccessor;
            timer = new Timer();
            timer.Elapsed += Elapsed;
            Logger = NullLogger.Instance;
            Interval = TimeSpan.FromMinutes(1);
        }

        public ILogger Logger { get; set; }

        public TimeSpan Interval
        {
            get { return TimeSpan.FromMilliseconds(timer.Interval); }
            set { timer.Interval = value.TotalMilliseconds; }
        }

        public void Activate()
        {
            lock (timer)
            {
                timer.Start();
            }
        }

        public void Terminate()
        {
            lock (timer)
            {
                timer.Stop();
            }
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            // current implementation disallows re-entrancy
            if (!System.Threading.Monitor.TryEnter(timer))
                return;

            try
            {
                if (timer.Enabled)
                {
                    DoWork();
                }
            }
            catch (Exception ex)
            {
                Logger.WarnFormat(ex, "Problem in background tasks");
            }
            finally
            {
                System.Threading.Monitor.Exit(timer);
            }
        }

        public void DoWork()
        {
            using (var scope = workContextAccessor.CreateWorkContextScope())
            {
                var transactionManager = scope.Resolve<ITransactionManager>();
                transactionManager.Demand();

                // resolve the manager and invoke it
                var manager = scope.Resolve<IBackgroundService>();
                manager.Sweep();
            }
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}