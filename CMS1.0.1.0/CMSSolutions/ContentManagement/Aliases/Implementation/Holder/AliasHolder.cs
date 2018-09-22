using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.ContentManagement.Aliases.Implementation.Map;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Aliases.Implementation.Holder
{
    [Feature(Constants.Areas.Aliases)]
    public class AliasHolder : IAliasHolder
    {
        public AliasHolder()
        {
            aliasMaps = new ConcurrentDictionary<string, AliasMap>(StringComparer.OrdinalIgnoreCase);
        }

        private readonly ConcurrentDictionary<string, AliasMap> aliasMaps;

        public void SetAliases(IEnumerable<AliasInfo> aliases)
        {
            var grouped = aliases.GroupBy(alias => alias.Area ?? String.Empty, StringComparer.InvariantCultureIgnoreCase);

            foreach (var group in grouped)
            {
                var map = GetMap(group.Key);

                foreach (var alias in group)
                {
                    map.Insert(alias);
                }
            }
        }

        public void SetAlias(AliasInfo alias)
        {
            foreach (var map in aliasMaps.Values)
            {
                map.Remove(alias);
            }

            GetMap(alias.Area).Insert(alias);
        }

        public IEnumerable<AliasMap> GetMaps()
        {
            return aliasMaps.Values;
        }

        public AliasMap GetMap(string areaName)
        {
            return aliasMaps.GetOrAdd(areaName ?? String.Empty, key => new AliasMap(key));
        }

        public void RemoveAlias(AliasInfo aliasInfo)
        {
            GetMap(aliasInfo.Area ?? String.Empty).Remove(aliasInfo);
        }
    }
}