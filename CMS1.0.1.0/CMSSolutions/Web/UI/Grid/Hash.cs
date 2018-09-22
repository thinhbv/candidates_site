using System;
using System.Collections.Generic;

namespace CMSSolutions.Web.UI.Grid
{
    internal class Hash : Hash<object>
    {
        public Hash(params Func<object, object>[] hash)
            : base(hash)
        {
        }
    }

    internal class Hash<TValue> : Dictionary<string, TValue>
    {
        public Hash(params Func<object, TValue>[] hash)
            : base((hash == null) ? 0 : hash.Length, StringComparer.OrdinalIgnoreCase)
        {
            if (hash != null)
            {
                foreach (Func<object, TValue> func in hash)
                {
                    Add(func.Method.GetParameters()[0].Name, func(null));
                }
            }
        }

        public static Dictionary<string, TValue> Empty
        {
            get
            {
                return new Dictionary<string, TValue>(0, StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}