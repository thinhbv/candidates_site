using System;
using System.Linq;
using Castle.Core.Logging;
using CMSSolutions.ContentManagement.Aliases.Implementation.Holder;
using CMSSolutions.ContentManagement.Aliases.Implementation.Storage;
using CMSSolutions.Environment;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Tasks;

namespace CMSSolutions.ContentManagement.Aliases.Implementation.Updater
{
    [Feature(Constants.Areas.Aliases)]
    public class AliasHolderUpdater : IShellEvents, IBackgroundTask
    {
        private readonly IAliasHolder aliasHolder;
        private readonly IAliasStorage storage;

        public ILogger Logger { get; set; }

        public AliasHolderUpdater(IAliasHolder aliasHolder, IAliasStorage storage)
        {
            this.aliasHolder = aliasHolder;
            this.storage = storage;
            Logger = NullLogger.Instance;
        }

        public int Priority { get { return 0; } }

        void IShellEvents.Activated()
        {
            Refresh();
        }

        void IShellEvents.Terminating()
        {
        }

        private void Refresh()
        {
            try
            {
                var aliases = storage.List();
                aliasHolder.SetAliases(aliases.Select(alias => new AliasInfo { Path = alias.Item1, Area = alias.Item2, RouteValues = alias.Item3 }));
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "Exception during Alias refresh");
            }
        }

        public void Sweep()
        {
            Refresh();
        }
    }
}