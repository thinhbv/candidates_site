using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Lists.Routing
{
    public interface IListCategoryPathConstraint : ISingletonDependency
    {
        bool Match(int listId, string slug, out int categoryId);

        void SetPaths(IEnumerable<ListCategory> categories);

        void AddPath(string path, int listId, int id);

        void RemovePath(string path, int listId);
    }

    [Feature(Constants.Areas.Lists)]
    public class ListCategoryPathConstraint : IListCategoryPathConstraint
    {
        private readonly ConcurrentDictionary<string, int> paths = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public bool Match(int listId, string slug, out int categoryId)
        {
            var key = slug + "/" + listId;
            if (paths.ContainsKey(key))
            {
                categoryId = paths[key];
                return true;
            }

            categoryId = 0;
            return false;
        }

        public void SetPaths(IEnumerable<ListCategory> categories)
        {
            paths.Clear();

            foreach (var category in categories)
            {
                var key = category.FullUrl + "/" + category.ListId;
                paths[key] = category.Id;
            }
        }

        public void AddPath(string path, int listId, int id)
        {
            var key = path + "/" + listId;
            paths[key] = id;
        }

        public void RemovePath(string path, int listId)
        {
            var key = path + "/" + listId;
            int id;
            paths.TryRemove(key, out id);
        }
    }
}