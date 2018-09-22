using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Castle.Core.Logging;
using CMSSolutions.Collections.Generic;
using CMSSolutions.Extensions;

namespace CMSSolutions.Collections
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Determines whether this collection contains any of the specified values
        /// </summary>
        /// <typeparam name="T">The type of the values to compare</typeparam>
        /// <param name="t">This collection</param>
        /// <param name="items">The values to compare</param>
        /// <returns>true if the collection contains any of the specified values, otherwise false</returns>
        public static bool ContainsAny<T>(this IEnumerable<T> t, params T[] items)
        {
            return items.Any(t.Contains);
        }

        public static bool ContainsAny<T>(this IEnumerable<T> t, IEnumerable<T> items)
        {
            return items.Any(t.Contains);
        }

        /// <summary>
        /// Performs the specified action on each element of the System.Collections.Generic.IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">This instance of System.Collections.Generic.IEnumerable&lt;T&gt;.</param>
        /// <param name="action">The System.Action&lt;T&gt; delegate to perform on each element of the System.Collections.Generic.IEnumerable&lt;T&gt;.</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }

        #region Invoke

        /// <summary>
        /// Safely invoke methods by catching non fatal exceptions and logging them
        /// </summary>
        public static void Invoke<TEvents>(this IEnumerable<TEvents> events, Action<TEvents> dispatch, ILogger logger)
        {
            foreach (var sink in events)
            {
                try
                {
                    dispatch(sink);
                }
                catch (Exception ex)
                {
                    if (IsLogged(ex))
                    {
                        logger.ErrorFormat(ex, "{2} thrown from {0} by {1}",
                            typeof(TEvents).Name,
                            sink.GetType().FullName,
                            ex.GetType().Name);
                    }

                    if (ex.IsFatal())
                    {
                        throw;
                    }
                }
            }
        }

        public static IEnumerable<TResult> Invoke<TEvents, TResult>(this IEnumerable<TEvents> events, Func<TEvents, TResult> dispatch, ILogger logger)
        {
            foreach (var sink in events)
            {
                TResult result = default(TResult);
                try
                {
                    result = dispatch(sink);
                }
                catch (Exception ex)
                {
                    if (IsLogged(ex))
                    {
                        logger.ErrorFormat(ex, "{2} thrown from {0} by {1}",
                            typeof(TEvents).Name,
                            sink.GetType().FullName,
                            ex.GetType().Name);
                    }

                    if (ex.IsFatal())
                    {
                        throw;
                    }
                }

                yield return result;
            }
        }

        private static bool IsLogged(Exception ex)
        {
            return ex is CMSException || !ex.IsFatal();
        }

        #endregion Invoke

        /// <summary>
        /// Indicates whether the specified System.Collections.Generic.IEnumerable&lt;T&gt; is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">This instance of System.Collections.Generic.IEnumerable&lt;T&gt;.</param>
        /// <returns>true if the System.Collections.Generic.IEnumerable&lt;T&gt; is null or empty; otherwise, false.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        /// <summary>
        /// <para>Returns all elements of this IEnumerable&lt;T&gt; in a single System.String.</para>
        /// <para>Elements are separated by a comma.</para>
        /// </summary>
        /// <param name="values">This instance of System.Collections.Generic.IEnumerable&lt;T&gt;.</param>
        /// <returns>System.String containing elements from specified IEnumerable&lt;T&gt;.</returns>
        public static string Join<T>(this IEnumerable<T> values)
        {
            return values.Join(",");
        }

        /// <summary>
        /// <para>Returns all elements of this IEnumerable&lt;T&gt; in a single System.String.</para>
        /// <para>Elements are separated by the specified separator.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">This instance of System.Collections.Generic.IEnumerable&lt;T&gt;.</param>
        /// <param name="separator">The System.String to use to separate each element.</param>
        /// <returns>System.String containing elements from specified IEnumerable&lt;T&gt;.</returns>
        public static string Join<T>(this IEnumerable<T> values, string separator)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (separator == null)
            {
                separator = string.Empty;
            }
            using (IEnumerator<T> enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return string.Empty;
                }

                var builder = new StringBuilder();
                if (!Equals(enumerator.Current, default(T)))
                {
                    builder.Append(enumerator.Current);
                }

                while (enumerator.MoveNext())
                {
                    builder.Append(separator);
                    if (!Equals(enumerator.Current, default(T)))
                    {
                        builder.Append(enumerator.Current);
                    }
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// <para>Returns all elements of this IEnumerable&lt;T&gt; in a single System.String.</para>
        /// <para>Elements are separated by a comma.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="enumerable">This instance of System.Collections.Generic.IEnumerable&lt;T&gt;.</param>
        /// <param name="selector"></param>
        /// <returns>System.String containing elements from specified IEnumerable&lt;T&gt;.</returns>
        public static string Join<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> selector)
        {
            return enumerable.Join(selector, ",");
        }

        /// <summary>
        /// <para>Returns all elements of this IEnumerable&lt;T&gt; in a single System.String.</para>
        /// <para>Elements are separated by the specified separator.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="enumerable">This instance of System.Collections.Generic.IEnumerable&lt;T&gt;.</param>
        /// <param name="selector"></param>
        /// <param name="separator"></param>
        /// <returns>System.String containing elements from specified IEnumerable&lt;T&gt;.</returns>
        public static string Join<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> selector, string separator)
        {
            return enumerable.Select(selector).Join(separator);
        }

        public static IEnumerable<T> SafeUnion<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            if (first.IsNullOrEmpty())
            {
                return second;
            }
            else if (second.IsNullOrEmpty())
            {
                return first;
            }
            return first.Union(second);
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.ToDataTable(string.Concat(typeof(T).Name, "_Table"));
        }

        /// <summary>
        /// Creates and returns a System.Data.DataTable from the specified System.Collections.Generic.IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">This instance of System.Collections.Generic.IEnumerable&lt;T&gt;.</param>
        /// <param name="tableName">The value to set for the DataTable's Name property.</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> enumerable, string tableName)
        {
            var table = new DataTable(tableName) { Locale = CultureInfo.InvariantCulture };

            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties();

            #region If T Is String Or Has No Properties

            if (properties.IsNullOrEmpty() || typeof(T) == typeof(string))
            {
                table.Columns.Add(new DataColumn("Value", typeof(string)));

                foreach (T item in enumerable)
                {
                    DataRow row = table.NewRow();

                    row["Value"] = item.ToString();

                    table.Rows.Add(row);
                }

                return table;
            }

            #endregion If T Is String Or Has No Properties

            #region Else Normal Collection

            foreach (PropertyInfo property in properties)
            {
                table.Columns.Add(new DataColumn(property.Name, property.PropertyType));
            }

            foreach (T item in enumerable)
            {
                DataRow row = table.NewRow();

                foreach (PropertyInfo property in properties)
                {
                    row[property.Name] = property.GetValue(item, null);
                }

                table.Rows.Add(row);
            }

            #endregion Else Normal Collection

            return table;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static List<T> ToListOf<T>(this IEnumerable enumerable)
        {
            return (from object item in enumerable select item.ConvertTo<T>()).ToList();
        }

        /// <summary>
        /// <para>Creates a CMSSolutions.Collections.Generic.PairCollection&lt;TFirst,TSecond&gt; from a System.Collections.Generic.IEnumerable&lt;T&gt;</para>
        /// <para>according to specified first and second selector functions.</para>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TFirst">The type of the First returned by firstSelector.</typeparam>
        /// <typeparam name="TSecond">The type of the Second returned by secondSelector.</typeparam>
        /// <param name="source">An System.Collections.Generic.IEnumerable&lt;T&gt; to create a CMSSolutions.Collections.Generic.PairCollection&lt;TFirst,TSecond&gt; from</param>
        /// <param name="firstSelector">A transform function to produce a result element First from each element.</param>
        /// <param name="secondSelector">A transform function to produce a result element Second from each element.</param>
        /// <returns>
        /// <para>A CMSSolutions.Collections.Generic.PairCollection&lt;TFirst,TSecond&gt; that contains values</para>
        /// <para> of type TFirst and TSecond selected from the input sequence.</para>
        /// </returns>
        public static PairList<TFirst, TSecond> ToPairList<TSource, TFirst, TSecond>(
            this IEnumerable<TSource> source,
            Func<TSource, TFirst> firstSelector,
            Func<TSource, TSecond> secondSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (firstSelector == null)
            {
                throw new ArgumentNullException("firstSelector");
            }
            if (secondSelector == null)
            {
                throw new ArgumentNullException("secondSelector");
            }
            var dictionary = new PairList<TFirst, TSecond>();
            foreach (TSource item in source)
            {
                dictionary.Add(firstSelector(item), secondSelector(item));
            }
            return dictionary;
        }

        public static IList<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            return new ReadOnlyCollection<T>(enumerable.ToList());
        }

        /// <summary>
        /// Creates a System.Collections.Generic.Queue&lt;T&gt; from an System.Collections.Generic.IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The System.Collections.Generic.IEnumerable&lt;T&gt; to create a System.Collections.Generic.Queue&lt;T&gt; from</param>
        /// <returns>A System.Collections.Generic.Queue&lt;T&gt; that contains elements from the input sequence</returns>
        public static Queue<TSource> ToQueue<TSource>(this IEnumerable<TSource> source)
        {
            var queue = new Queue<TSource>();
            foreach (TSource item in source)
            {
                queue.Enqueue(item);
            }
            return queue;
        }

        /// <summary>
        /// Creates a System.Collections.Generic.Stack&lt;T&gt; from an System.Collections.Generic.IEnumerable&lt;T&gt;.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The System.Collections.Generic.IEnumerable&lt;T&gt; to create a System.Collections.Generic.Stack&lt;T&gt; from</param>
        /// <returns>A System.Collections.Generic.Stack&lt;T&gt; that contains elements from the input sequence</returns>
        public static Stack<TSource> ToStack<TSource>(this IEnumerable<TSource> source)
        {
            var stack = new Stack<TSource>();
            foreach (TSource item in source.Reverse())
            {
                stack.Push(item);
            }
            return stack;
        }

        /// <summary>
        /// <para>Creates a CMSSolutions.Collections.Generic.TripleCollection&lt;TFirst,TSecond,TThird&gt; from a System.Collections.Generic.IEnumerable&lt;T&gt;</para>
        /// <para>according to specified first and second selector functions.</para>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TFirst">The type of the First returned by firstSelector.</typeparam>
        /// <typeparam name="TSecond">The type of the Second returned by secondSelector.</typeparam>
        /// <typeparam name="TThird">The type of the Third returned by thirdSelector.</typeparam>
        /// <param name="source"></param>
        /// <param name="firstSelector">A transform function to produce a result element First from each element.</param>
        /// <param name="secondSelector">A transform function to produce a result element Second from each element.</param>
        /// <param name="thirdSelector">A transform function to produce a result element Third from each element.</param>
        /// <returns>
        /// <para>A CMSSolutions.Collections.Generic.TripleCollection&lt;TFirst,TSecond,TThird&gt; that contains values</para>
        /// <para> of type TFirst, TSecond and TThird selected from the input sequence.</para>
        /// </returns>
        public static TripleList<TFirst, TSecond, TThird> ToTripleList<TSource, TFirst, TSecond, TThird>(
            this IEnumerable<TSource> source,
            Func<TSource, TFirst> firstSelector,
            Func<TSource, TSecond> secondSelector,
            Func<TSource, TThird> thirdSelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (firstSelector == null)
            {
                throw new ArgumentNullException("firstSelector");
            }
            if (secondSelector == null)
            {
                throw new ArgumentNullException("secondSelector");
            }
            if (thirdSelector == null)
            {
                throw new ArgumentNullException("thirdSelector");
            }
            var dictionary = new TripleList<TFirst, TSecond, TThird>();
            foreach (TSource item in source)
            {
                dictionary.Add(firstSelector(item), secondSelector(item), thirdSelector(item));
            }
            return dictionary;
        }

        public static SelectList ToSelectList(this IEnumerable<string> enumerable)
        {
            return enumerable.ToSelectList(x => x, x => x);
        }

        public static SelectList ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, object> valueFieldSelector, Func<T, string> textFieldSelector)
        {
            var values = from T item in enumerable
                         select new
                         {
                             ValueField = Convert.ToString(valueFieldSelector(item)),
                             TextField = textFieldSelector(item)
                         };
            return new SelectList(values, "ValueField", "TextField");
        }

        public static SelectList ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> valueFieldSelector, Func<T, string> textFieldSelector, string emptyText)
        {
            var values = (from T item in enumerable
                          select new
                              {
                                  ValueField = valueFieldSelector(item),
                                  TextField = textFieldSelector(item)
                              }).ToList();

            if (emptyText != null) // we don't check for empty, because empty string can be valid for emptyText value.
            {
                values.Insert(0, new { ValueField = "", TextField = emptyText });
            }

            return new SelectList(values, "ValueField", "TextField");
        }

        public static SelectList ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> valueFieldSelector, Func<T, string> textFieldSelector, object selectedValue)
        {
            var values = from T item in enumerable
                         select new
                         {
                             ValueField = valueFieldSelector(item),
                             TextField = textFieldSelector(item)
                         };
            return new SelectList(values, "ValueField", "TextField", selectedValue);
        }

        public static SelectList ToSelectList<T>(this IEnumerable<T> enumerable, Func<T, string> valueFieldSelector, Func<T, string> textFieldSelector, object selectedValue, string emptyText)
        {
            var values = (from T item in enumerable
                          select new
                          {
                              ValueField = valueFieldSelector(item),
                              TextField = textFieldSelector(item)
                          }).ToList();

            if (emptyText != null) // we don't check for empty, because empty string can be valid for emptyText value.
            {
                values.Insert(0, new { ValueField = "", TextField = emptyText });
            }
            return new SelectList(values, "ValueField", "TextField", selectedValue);
        }

        public static MultiSelectList ToMultiSelectList<T, TValue>(this IEnumerable<T> enumerable, Func<T, string> valueFieldSelector, Func<T, string> textFieldSelector, IEnumerable<TValue> selectedValues, string emptyText = null)
        {
            var values = (from T item in enumerable
                          select new
                          {
                              ValueField = valueFieldSelector(item),
                              TextField = textFieldSelector(item)
                          }).ToList();

            if (emptyText != null) // we don't check for empty, because empty string can be valid for emptyText value.
            {
                values.Insert(0, new { ValueField = "", TextField = emptyText });
            }

            return new MultiSelectList(values, "ValueField", "TextField", selectedValues);
        }

        public static IEnumerable<T> Descendants<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> descendBy)
        {
            foreach (T value in source)
            {
                yield return value;

                foreach (T child in descendBy(value).Descendants<T>(descendBy))
                {
                    yield return child;
                }
            }
        }
    }
}