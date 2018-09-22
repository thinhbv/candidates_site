using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using CMSSolutions.Collections;

namespace CMSSolutions.Extensions
{
    public static class EnumExtensions
    {
        public static List<SelectListItem> GetListItems<T>()
        {
            var results = new List<SelectListItem>();
            foreach (T key in (T[])Enum.GetValues(typeof(T)))
            {
                var type = typeof(T);
                var member = type.GetMember(key.ToString());
                var attributes = member[0].GetCustomAttributes(typeof(DisplayAttribute), false);
                var name = ((DisplayAttribute)attributes[0]).Name;

                results.Add(new SelectListItem
                {
                    Value = Convert.ToUInt32(key).ToString(),
                    Text = name
                });
            }

            return results;
        }

        public static T ToEnum<T>(this string str) where T : struct
        {
            return (T)Enum.Parse(typeof(T), str);
        }

        public static T ToEnum<T>(this string str, bool ignoreCase) where T : struct
        {
            return (T)Enum.Parse(typeof(T), str, ignoreCase);
        }

        public static T[] GetValues<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        public static T Parse<T>(string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static IEnumerable<string> GetValuesAsWords<T>()
        {
            var values = new List<string>();
            GetValues<T>().ForEach(item => values.Add(item.ToString().SpacePascal()));
            return values;
        }

        public static IEnumerable<Enum> GetFlags(this Enum value)
        {
            if (value == null)
            {
                return Enumerable.Empty<Enum>();
            }

            return GetFlags(value, Enum.GetValues(value.GetType()).Cast<Enum>().ToArray());
        }

        public static IEnumerable<Enum> GetIndividualFlags(this Enum value)
        {
            if (value == null)
            {
                return Enumerable.Empty<Enum>();
            }

            return GetFlags(value, GetFlagValues(value.GetType()).ToArray());
        }

        private static IEnumerable<Enum> GetFlags(Enum value, Enum[] values)
        {
            ulong bits = Convert.ToUInt64(value);
            var results = new List<Enum>();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                ulong mask = Convert.ToUInt64(values[i]);
                if (i == 0 && mask == 0L)
                    break;
                if ((bits & mask) == mask)
                {
                    results.Add(values[i]);
                    bits -= mask;
                }
            }
            if (bits != 0L)
                return Enumerable.Empty<Enum>();
            if (Convert.ToUInt64(value) != 0L)
                return results.Reverse<Enum>();
            if (bits == Convert.ToUInt64(value) && values.Length > 0 && Convert.ToUInt64(values[0]) == 0L)
                return values.Take(1);
            return Enumerable.Empty<Enum>();
        }

        private static IEnumerable<Enum> GetFlagValues(Type enumType)
        {
            ulong flag = 0x1;
            foreach (var value in Enum.GetValues(enumType).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);
                if (bits == 0L)

                    //yield return value;
                    continue; // skip the zero value
                while (flag < bits) flag <<= 1;
                if (flag == bits)
                    yield return value;
            }
        }

        //Why are these versions here with "Type"? Why did they replace the generic versions?

        public static SelectList ToSelectList(this Type type)
        {
            return ToSelectList(type, null);
        }

        public static SelectList ToSelectList(this Type type, object selectedValue, string emptyText = null)
        {
            if (!type.IsEnum)
            {
                throw new NotSupportedException("The type must be is enum type.");
            }

            var array = Enum.GetValues(type);
            int order;
            var values = (from object e in array
                         select new
                         {
                             Id = e.ConvertTo<int>(),
                             Name = GetDisplayName(e, out order),
                             Order = order
                         }).ToList();

            values = values
                .Where(x => x.Order != -1)
                .OrderBy(x => x.Order)
                .ToList();

            if (emptyText != null)
            {
                values.Insert(0, new { Id = -1, Name = emptyText, Order = -1 });
            }

            return new SelectList(values, "Id", "Name", selectedValue);
        }

        public static SelectList ToSelectList<T>() where T : struct
        {
            return ToSelectList<T>(null);
        }

        public static SelectList ToSelectList<T>(object selectedValue, string emptyText = null) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException("You must specify an enum type");
            }

            int order;
            var values = (from T e in GetValues<T>()
                         select new
                         {
                             Id = e.ConvertTo<int>(),
                             Name = GetDisplayName(e, out order),
                             Order = order
                         }).ToList();

            values = values
                .Where(x => x.Order != -1)
                .OrderBy(x => x.Order)
                .ToList();

            if (emptyText != null)
            {
                values.Insert(0, new { Id = -1, Name = emptyText, Order = -1 });
            }

            return new SelectList(values, "Id", "Name", selectedValue);
        }

        public static MultiSelectList ToMultiSelectList(this Type type)
        {
            return ToMultiSelectList(type, null);
        }

        public static MultiSelectList ToMultiSelectList(this Type type, IEnumerable selectedValues, string emptyText = null)
        {
            if (!type.IsEnum)
            {
                throw new NotSupportedException("The type must be is enum type.");
            }

            var array = Enum.GetValues(type);
            int order;
            var values = (from object e in array
                         select new
                         {
                             Id = e.ConvertTo<int>(),
                             Name = GetDisplayName(e, out order),
                             Order = order
                         }).ToList();

            values = values
                .Where(x => x.Order != -1)
                .OrderBy(x => x.Order)
                .ToList();

            if (emptyText != null)
            {
                values.Insert(0, new { Id = -1, Name = emptyText, Order = -1 });
            }

            return new MultiSelectList(values, "Id", "Name", selectedValues);
        }

        public static MultiSelectList ToMultiSelectList<T>() where T : struct
        {
            return ToMultiSelectList<T>(null);
        }

        public static MultiSelectList ToMultiSelectList<T>(IEnumerable selectedValues, string emptyText = null) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException("You must specify an enum type");
            }

            int order;
            var values = (from T e in GetValues<T>()
                         select new
                         {
                             Id = e.ConvertTo<int>(),
                             Name = GetDisplayName(e, out order),
                             Order = order
                         }).ToList();

            values = values
                .Where(x => x.Order != -1)
                .OrderBy(x => x.Order)
                .ToList();

            if (emptyText != null)
            {
                values.Insert(0, new { Id = -1, Name = emptyText, Order = -1 });
            }

            return new MultiSelectList(values, "Id", "Name", selectedValues);
        }

        public static string GetDisplayName<T>(T value)
        {
            if (!(value is Enum))
            {
                return value.ToString();
            }

            var displayAttribute = typeof(T).GetField(value.ToString()).GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            if (displayAttribute != null)
            {
                var attribute = (DisplayAttribute)displayAttribute;

                string displayName = attribute.Name;
                if (string.IsNullOrEmpty(displayName))
                {
                    return value.ToString().SpacePascal();
                }

                return displayName;
            }

            return value.ToString();
        }

        private static string GetDisplayName<T>(T value, out int order)
        {
            order = -1;
            if (!(value is Enum))
            {
                return value.ToString();
            }

            var displayAttribute = typeof(T).GetField(value.ToString()).GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            if (displayAttribute != null)
            {
                var attribute = (DisplayAttribute)displayAttribute;

                int? displayOrder = attribute.GetOrder();
                order = displayOrder ?? 0;

                string displayName = attribute.Name;
                if (string.IsNullOrEmpty(displayName))
                {
                    return value.ToString().SpacePascal();
                }

                return displayName;
            }

            return value.ToString();
        }
    }
}