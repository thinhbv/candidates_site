using System;
using System.Collections.Generic;
using System.Linq;

namespace CMSSolutions.Collections.Generic
{
    /// <summary>
    /// Hierarchy node class which contains a nested collection of hierarchy nodes
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    public class HierarchyNode<T> where T : class
    {
        public T Entity { get; set; }

        public IEnumerable<HierarchyNode<T>> ChildNodes { get; set; }

        public int Depth { get; set; }

        public T Parent { get; set; }
    }

    public static class EnumerableHierarchyExtensions
    {
        private static IEnumerable<HierarchyNode<TEntity>> CreateHierarchy<TEntity, TProperty>(
            IEnumerable<TEntity> items,
            TEntity parentItem,
            Func<TEntity, TProperty> idSelector,
            Func<TEntity, TProperty> parentIdSelector,
            object rootItemId,
            int maxDepth,
            int depth) where TEntity : class
        {
            IEnumerable<TEntity> children;

            if (rootItemId != null)
            {
                children = items.Where(i => idSelector(i).Equals(rootItemId));
            }
            else
            {
                if (parentItem == null)
                {
                    children = items.Where(i => parentIdSelector(i).Equals(default(TProperty)));
                }
                else
                {
                    children = items.Where(i => parentIdSelector(i).Equals(idSelector(parentItem)));
                }
            }

            if (children.Any())
            {
                depth++;

                if (depth <= maxDepth || maxDepth == 0)
                {
                    foreach (var item in children)
                    {
                        yield return new HierarchyNode<TEntity>()
                        {
                            Entity = item,
                            ChildNodes =
                            CreateHierarchy(items.AsEnumerable(), item, idSelector, parentIdSelector, null, maxDepth, depth),
                            Depth = depth,
                            Parent = parentItem
                        };
                    }
                }
            }
        }

        /// <summary>
        /// LINQ to Objects (IEnumerable) AsHierachy() extension method
        /// </summary>
        /// <typeparam name="TEntity">Entity class</typeparam>
        /// <typeparam name="TProperty">Property of entity class</typeparam>
        /// <param name="items">Flat collection of entities</param>
        /// <param name="idSelector">Func delegete to Id/Key of entity</param>
        /// <param name="parentIdSelector">Func delegete to parent Id/Key</param>
        /// <returns>Hierarchical structure of entities</returns>
        public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity, TProperty>(
          this IEnumerable<TEntity> items,
          Func<TEntity, TProperty> idSelector,
          Func<TEntity, TProperty> parentIdSelector) where TEntity : class
        {
            return CreateHierarchy(items, default(TEntity), idSelector, parentIdSelector, null, 0, 0);
        }

        /// <summary>
        /// LINQ to Objects (IEnumerable) AsHierachy() extension method
        /// </summary>
        /// <typeparam name="TEntity">Entity class</typeparam>
        /// <typeparam name="TProperty">Property of entity class</typeparam>
        /// <param name="items">Flat collection of entities</param>
        /// <param name="idSelector">Func delegete to Id/Key of entity</param>
        /// <param name="parentIdSelector">Func delegete to parent Id/Key</param>
        /// <param name="rootItemId">Value of root item Id/Key</param>
        /// <returns>Hierarchical structure of entities</returns>
        public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity, TProperty>(
          this IEnumerable<TEntity> items,
          Func<TEntity, TProperty> idSelector,
          Func<TEntity, TProperty> parentIdSelector,
          object rootItemId) where TEntity : class
        {
            return CreateHierarchy(items, default(TEntity), idSelector, parentIdSelector, rootItemId, 0, 0);
        }

        /// <summary>
        /// LINQ to Objects (IEnumerable) AsHierachy() extension method
        /// </summary>
        /// <typeparam name="TEntity">Entity class</typeparam>
        /// <typeparam name="TProperty">Property of entity class</typeparam>
        /// <param name="items">Flat collection of entities</param>
        /// <param name="idSelector">Func delegete to Id/Key of entity</param>
        /// <param name="parentIdSelector">Func delegete to parent Id/Key</param>
        /// <param name="rootItemId">Value of root item Id/Key</param>
        /// <param name="maxDepth">Maximum depth of tree</param>
        /// <returns>Hierarchical structure of entities</returns>
        public static IEnumerable<HierarchyNode<TEntity>> AsHierarchy<TEntity, TProperty>(
          this IEnumerable<TEntity> items,
          Func<TEntity, TProperty> idSelector,
          Func<TEntity, TProperty> parentIdSelector,
          object rootItemId,
          int maxDepth) where TEntity : class
        {
            return CreateHierarchy(items, default(TEntity), idSelector, parentIdSelector, rootItemId, maxDepth, 0);
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childSelector)
        {
            return source.SelectMany(x => Flatten(childSelector(x), childSelector)).Concat(source);
        }
    }
}