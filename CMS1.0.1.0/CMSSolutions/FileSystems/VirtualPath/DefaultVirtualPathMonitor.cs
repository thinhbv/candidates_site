using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using Castle.Core.Logging;
using CMSSolutions.Caching;

namespace CMSSolutions.FileSystems.VirtualPath
{
    public class DefaultVirtualPathMonitor : IVirtualPathMonitor
    {
        private readonly Thunk thunk;
        private readonly string prefix = Guid.NewGuid().ToString("n");
        private readonly IDictionary<string, Weak<Token>> tokens = new Dictionary<string, Weak<Token>>();

        public DefaultVirtualPathMonitor()
        {
            thunk = new Thunk(this);
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public IVolatileToken WhenPathChanges(string virtualPath)
        {
            var token = BindToken(virtualPath);
            try
            {
                BindSignal(virtualPath);
            }
            catch (HttpException e)
            {
                // This exception happens if trying to monitor a directory or file
                // inside a directory which doesn't exist
                Logger.InfoFormat(e, "Error monitoring file changes on virtual path '{0}'", virtualPath);
            }
            return token;
        }

        private Token BindToken(string virtualPath)
        {
            lock (tokens)
            {
                Weak<Token> weak;
                if (!tokens.TryGetValue(virtualPath, out weak))
                {
                    weak = new Weak<Token>(new Token(virtualPath));
                    tokens[virtualPath] = weak;
                }

                var token = weak.Target;
                if (token == null)
                {
                    token = new Token(virtualPath);
                    weak.Target = token;
                }

                return token;
            }
        }

        private Token DetachToken(string virtualPath)
        {
            lock (tokens)
            {
                Weak<Token> weak;
                if (!tokens.TryGetValue(virtualPath, out weak))
                {
                    return null;
                }
                var token = weak.Target;
                weak.Target = null;
                return token;
            }
        }

        private void BindSignal(string virtualPath)
        {
            BindSignal(virtualPath, thunk.Signal);
        }

        private void BindSignal(string virtualPath, CacheItemRemovedCallback callback)
        {
            string key = prefix + virtualPath;

            //PERF: Don't add in the cache if already present. Creating a "CacheDependency"
            //      object (below) is actually quite expensive.
            if (HostingEnvironment.Cache.Get(key) != null)
                return;

            var cacheDependency = HostingEnvironment.VirtualPathProvider.GetCacheDependency(
                virtualPath,
                new[] { virtualPath },
                DateTime.UtcNow);

            HostingEnvironment.Cache.Add(
                key,
                virtualPath,
                cacheDependency,
                Cache.NoAbsoluteExpiration,
                Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable,
                callback);
        }

        public void Signal(string key, object value, CacheItemRemovedReason reason)
        {
            var virtualPath = Convert.ToString(value);
            var token = DetachToken(virtualPath);
            if (token != null)
                token.IsCurrent = false;
        }

        public class Token : IVolatileToken
        {
            public Token(string virtualPath)
            {
                IsCurrent = true;
                VirtualPath = virtualPath;
            }

            public bool IsCurrent { get; set; }

            public string VirtualPath { get; private set; }

            public override string ToString()
            {
                return string.Format("IsCurrent: {0}, VirtualPath: \"{1}\"", IsCurrent, VirtualPath);
            }
        }

        private class Thunk
        {
            private readonly Weak<DefaultVirtualPathMonitor> weak;

            public Thunk(DefaultVirtualPathMonitor provider)
            {
                weak = new Weak<DefaultVirtualPathMonitor>(provider);
            }

            public void Signal(string key, object value, CacheItemRemovedReason reason)
            {
                var provider = weak.Target;
                if (provider != null)
                    provider.Signal(key, value, reason);
            }
        }
    }
}