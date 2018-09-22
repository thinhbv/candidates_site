#region License

/*
 * All content copyright Terracotta, Inc., unless otherwise indicated. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */

#endregion License

using System;
using System.Collections;
using System.Collections.Generic;

namespace CMSSolutions.Quartz.Collection
{
    [Serializable]
    public class TreeSet<T> : ISortedSet<T>
    {
        private readonly SortedSet<T> sortedSet;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TreeSet()
        {
            sortedSet = new SortedSet<T>();
        }

        /// <summary>
        /// Constructor that accepts comparer.
        /// </summary>
        /// <param name="comparer">Comparer to use.</param>
        public TreeSet(IComparer<T> comparer)
        {
            sortedSet = new SortedSet<T>(comparer);
        }

        /// <summary>
        /// Constructor that prepolutates.
        /// </summary>
        /// <param name="items"></param>
        public TreeSet(IEnumerable<T> items)
        {
            sortedSet = new SortedSet<T>();

            foreach (T item in items)
            {
                sortedSet.Add(item);
            }
        }

        public TreeSet(SortedSet<T> sortedSet)
        {
            this.sortedSet = sortedSet;
        }

        public void AddAll(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                sortedSet.Add(item);
            }
        }

        /// <summary>
        /// Returns the first element.
        /// </summary>
        /// <returns></returns>
        public T First()
        {
            return sortedSet.Count > 0 ? sortedSet.Min : default(T);
        }

        /// <summary>
        /// Return items from given range.
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ISortedSet<T> TailSet(T limit)
        {
            if (sortedSet.Comparer.Compare(sortedSet.Max, limit) == -1)
            {
                return new TreeSet<T>();
            }

            var item = sortedSet.GetViewBetween(limit, sortedSet.Max);
            return new TreeSet<T>(item);
        }

        public T this[int index]
        {
            get
            {
                var list = new List<T>();
                var enumerator = GetEnumerator();
                while (enumerator.MoveNext())
                {
                    list.Add(enumerator.Current);
                }
                return list[index];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return sortedSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            sortedSet.Add(item);
        }

        public void Clear()
        {
            sortedSet.Clear();
        }

        public bool Contains(T item)
        {
            return sortedSet.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            sortedSet.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return sortedSet.Remove(item);
        }

        public int Count
        {
            get
            {
                return sortedSet.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
    }
}