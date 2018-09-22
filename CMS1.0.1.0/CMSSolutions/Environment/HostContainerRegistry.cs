using System.Collections.Generic;
using CMSSolutions.Caching;

namespace CMSSolutions.Environment
{
    /// <summary>
    /// Provides ability to connect Shims to the HostContainer
    /// </summary>
    public static class HostContainerRegistry
    {
        private static readonly IList<Weak<IShim>> shims = new List<Weak<IShim>>();
        private static IHostContainer hostContainer;
        private static readonly object syncLock = new object();

        public static void RegisterShim(IShim shim)
        {
            lock (syncLock)
            {
                CleanupShims();

                shims.Add(new Weak<IShim>(shim));
                shim.HostContainer = hostContainer;
            }
        }

        public static void RegisterHostContainer(IHostContainer container)
        {
            lock (syncLock)
            {
                CleanupShims();

                hostContainer = container;
                RegisterContainerInShims();
            }
        }

        private static void RegisterContainerInShims()
        {
            foreach (var shim in shims)
            {
                var target = shim.Target;
                if (target != null)
                {
                    target.HostContainer = hostContainer;
                }
            }
        }

        private static void CleanupShims()
        {
            for (int i = shims.Count - 1; i >= 0; i--)
            {
                if (shims[i].Target == null)
                    shims.RemoveAt(i);
            }
        }
    }
}