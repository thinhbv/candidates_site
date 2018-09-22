using System;
using System.Collections.Concurrent;
using System.Web.Mvc;

namespace CMSSolutions.Web.Mvc.ViewEngines.ThemeAwareness
{
    public interface IConfiguredEnginesCache : ISingletonDependency
    {
        IViewEngine BindBareEngines(Func<IViewEngine> factory);

        IViewEngine BindShallowEngines(string themeName, Func<IViewEngine> factory);

        IViewEngine BindDeepEngines(string themeName, Func<IViewEngine> factory);
    }

    public class ConfiguredEnginesCache : IConfiguredEnginesCache
    {
        private IViewEngine bare;
        private readonly ConcurrentDictionary<string, IViewEngine> shallow = new ConcurrentDictionary<string, IViewEngine>();
        private readonly ConcurrentDictionary<string, IViewEngine> deep = new ConcurrentDictionary<string, IViewEngine>();

        public ConfiguredEnginesCache()
        {
            shallow = new ConcurrentDictionary<string, IViewEngine>();
        }

        public IViewEngine BindBareEngines(Func<IViewEngine> factory)
        {
            return bare ?? (bare = factory());
        }

        public IViewEngine BindShallowEngines(string themeName, Func<IViewEngine> factory)
        {
            return shallow.GetOrAdd(themeName, key => factory());
        }

        public IViewEngine BindDeepEngines(string themeName, Func<IViewEngine> factory)
        {
            return deep.GetOrAdd(themeName, key => factory());
        }
    }
}