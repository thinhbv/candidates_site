using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Data;
using CMSSolutions.Environment.Descriptor.Models;
using CMSSolutions.Environment.ShellBuilders;
using CMSSolutions.Events;

namespace CMSSolutions.Environment.State
{
    public class DefaultProcessingEngine : Component, IProcessingEngine
    {
        private readonly IShellContextFactory shellContextFactory;
        private readonly Func<ICMSHost> CMSHost;
        private readonly IList<Entry> entries = new List<Entry>();

        public DefaultProcessingEngine(IShellContextFactory shellContextFactory, Func<ICMSHost> CMSHost)
        {
            this.shellContextFactory = shellContextFactory;
            this.CMSHost = CMSHost;
        }

        public string AddTask(ShellSettings shellSettings, ShellDescriptor shellDescriptor, string eventName, Dictionary<string, object> parameters)
        {
            var entry = new Entry
            {
                ShellSettings = shellSettings,
                ShellDescriptor = shellDescriptor,
                MessageName = eventName,
                EventData = parameters,
                TaskId = Guid.NewGuid().ToString("n"),
                ProcessId = Guid.NewGuid().ToString("n"),
            };

            Logger.InfoFormat("Adding event {0} to process {1} for shell {2}",
                eventName,
                entry.ProcessId,
                shellSettings.Name);
            lock (entries)
            {
                entries.Add(entry);
                return entry.ProcessId;
            }
        }

        public class Entry
        {
            public string ProcessId { get; set; }

            public string TaskId { get; set; }

            public ShellSettings ShellSettings { get; set; }

            public ShellDescriptor ShellDescriptor { get; set; }

            public string MessageName { get; set; }

            public Dictionary<string, object> EventData { get; set; }
        }

        public bool AreTasksPending()
        {
            lock (entries)
                return entries.Any();
        }

        public bool AreTasksPending(string messageName)
        {
            lock (entries)
            {
                return entries.Any(x => x.MessageName == messageName);
            }
        }

        public void ExecuteNextTask()
        {
            Entry entry;
            lock (entries)
            {
                if (!entries.Any())
                    return;
                entry = entries.First();
                entries.Remove(entry);
            }
            Execute(entry);
        }

        private void Execute(Entry entry)
        {
            // Force reloading extensions if there were extensions installed
            if (entry.MessageName == "IRecipeSchedulerEventHandler.ExecuteWork")
            {
                var ctx = CMSHost().GetShellContext(entry.ShellSettings);
            }

            var shellContext = shellContextFactory.CreateDescribedContext(entry.ShellSettings, entry.ShellDescriptor);
            using (shellContext.LifetimeScope)
            {
                using (var standaloneEnvironment = shellContext.LifetimeScope.CreateWorkContextScope())
                {
                    ITransactionManager transactionManager;
                    if (!standaloneEnvironment.TryResolve(out transactionManager))
                        transactionManager = null;

                    try
                    {
                        var eventBus = standaloneEnvironment.Resolve<IEventBus>();
                        Logger.InfoFormat("Executing event {0} in process {1} for shell {2}",
                                           entry.MessageName,
                                           entry.ProcessId,
                                           entry.ShellSettings.Name);
                        eventBus.Notify(entry.MessageName, entry.EventData);
                    }
                    catch
                    {
                        // any database changes in this using(env) scope are invalidated
                        if (transactionManager != null)
                            transactionManager.Cancel();
                        throw;
                    }
                }
            }
        }
    }
}