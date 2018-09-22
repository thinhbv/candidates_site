using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CMSSolutions.ContentManagement.Aliases.Domain;
using CMSSolutions.ContentManagement.Aliases.Implementation.Holder;
using CMSSolutions.Data;
using CMSSolutions.Environment.Extensions;
using Action = CMSSolutions.ContentManagement.Aliases.Domain.Action;

namespace CMSSolutions.ContentManagement.Aliases.Implementation.Storage
{
    public interface IAliasStorage : IDependency
    {
        void Set(string path, IDictionary<string, string> routeValues, string source);

        IDictionary<string, string> Get(string aliasPath);

        void Remove(string path);

        void Remove(string path, string aliasSource);

        void RemoveBySource(string aliasSource);

        IEnumerable<Tuple<string, string, IDictionary<string, string>, string>> List();

        IEnumerable<Tuple<string, string, IDictionary<string, string>, string>> List(string sourceStartsWith);
    }

    [Feature(Constants.Areas.Aliases)]
    public class AliasStorage : IAliasStorage
    {
        private readonly IRepository<Alias, int> aliasRepository;
        private readonly IRepository<Action, int> actionRepository;
        private readonly IAliasHolder aliasHolder;

        public AliasStorage(IRepository<Alias, int> aliasRepository, IRepository<Action, int> actionRepository, IAliasHolder aliasHolder)
        {
            this.aliasRepository = aliasRepository;
            this.actionRepository = actionRepository;
            this.aliasHolder = aliasHolder;
        }

        public void Set(string path, IDictionary<string, string> routeValues, string source)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            var aliasRecord = aliasRepository.Table.FirstOrDefault(x => x.Path == path);
            aliasRecord = aliasRecord ?? new Alias { Path = path };

            string areaName = null;
            string controllerName = null;
            string actionName = null;
            var values = new XElement("v");
            foreach (var routeValue in routeValues.OrderBy(kv => kv.Key, StringComparer.InvariantCultureIgnoreCase))
            {
                if (string.Equals(routeValue.Key, "area", StringComparison.InvariantCultureIgnoreCase)
                    || string.Equals(routeValue.Key, "area-", StringComparison.InvariantCultureIgnoreCase))
                {
                    areaName = routeValue.Value;
                }
                else if (string.Equals(routeValue.Key, "controller", StringComparison.InvariantCultureIgnoreCase))
                {
                    controllerName = routeValue.Value;
                }
                else if (string.Equals(routeValue.Key, "action", StringComparison.InvariantCultureIgnoreCase))
                {
                    actionName = routeValue.Value;
                }
                else
                {
                    values.SetAttributeValue(routeValue.Key, routeValue.Value);
                }
            }

            aliasRecord.Action = actionRepository.Table.FirstOrDefault(r => r.Area == areaName && r.Controller == controllerName && r.ActionName == actionName);
            aliasRecord.Action = aliasRecord.Action ?? new Action { Area = areaName, Controller = controllerName, ActionName = actionName };

            aliasRecord.RouteValues = values.ToString();
            aliasRecord.Source = source;
            if (aliasRecord.Action.Id == 0 || aliasRecord.Id == 0)
            {
                if (aliasRecord.Action.Id == 0)
                {
                    actionRepository.Insert(aliasRecord.Action);
                }

                if (aliasRecord.Id == 0)
                {
                    aliasRepository.Insert(aliasRecord);
                }
            }
            // Transform and push into AliasHolder
            var dict = ToDictionary(aliasRecord);
            aliasHolder.SetAlias(new AliasInfo { Path = dict.Item1, Area = dict.Item2, RouteValues = dict.Item3 });
        }

        public IDictionary<string, string> Get(string path)
        {
            return aliasRepository
                .Table.Where(r => r.Path == path)
                .Select(ToDictionary)
                .Select(item => item.Item3)
                .SingleOrDefault();
        }

        public void Remove(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            foreach (var aliasRecord in aliasRepository.Table.Where(r => r.Path == path).ToList())
            {
                aliasRepository.Delete(aliasRecord);
                var dict = ToDictionary(aliasRecord);
                aliasHolder.RemoveAlias(new AliasInfo { Path = dict.Item1, Area = dict.Item2, RouteValues = dict.Item3 });
            }
        }

        public void Remove(string path, string aliasSource)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            foreach (var aliasRecord in aliasRepository.Table.Where(r => r.Path == path && r.Source == aliasSource).ToList())
            {
                aliasRepository.Delete(aliasRecord);
                var dict = ToDictionary(aliasRecord);
                aliasHolder.RemoveAlias(new AliasInfo { Path = dict.Item1, Area = dict.Item2, RouteValues = dict.Item3 });
            }
        }

        public void RemoveBySource(string aliasSource)
        {
            foreach (var aliasRecord in aliasRepository.Table.Where(r => r.Source == aliasSource).ToList())
            {
                aliasRepository.Delete(aliasRecord);
                var dict = ToDictionary(aliasRecord);
                aliasHolder.RemoveAlias(new AliasInfo { Path = dict.Item1, Area = dict.Item2, RouteValues = dict.Item3 });
            }
        }

        public IEnumerable<Tuple<string, string, IDictionary<string, string>, string>> List()
        {
            return aliasRepository.Table.OrderBy(a => a.Id).Select(ToDictionary).ToList();
        }

        public IEnumerable<Tuple<string, string, IDictionary<string, string>, string>> List(string sourceStartsWith)
        {
            return aliasRepository.Table.Where(a => a.Source.StartsWith(sourceStartsWith)).OrderBy(a => a.Id).Select(ToDictionary).ToList();
        }

        private static Tuple<string, string, IDictionary<string, string>, string> ToDictionary(Alias aliasRecord)
        {
            IDictionary<string, string> routeValues = new Dictionary<string, string>();
            if (aliasRecord.Action.Area != null)
            {
                routeValues.Add("area", aliasRecord.Action.Area);
            }
            if (aliasRecord.Action.Controller != null)
            {
                routeValues.Add("controller", aliasRecord.Action.Controller);
            }
            if (aliasRecord.Action.ActionName != null)
            {
                routeValues.Add("action", aliasRecord.Action.ActionName);
            }
            if (!string.IsNullOrEmpty(aliasRecord.RouteValues))
            {
                foreach (var attr in XElement.Parse(aliasRecord.RouteValues).Attributes())
                {
                    routeValues.Add(attr.Name.LocalName, attr.Value);
                }
            }
            return Tuple.Create(aliasRecord.Path, aliasRecord.Action.Area, routeValues, aliasRecord.Source);
        }
    }
}