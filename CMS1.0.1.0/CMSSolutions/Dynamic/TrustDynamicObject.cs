using System.Collections.Generic;
using System.Dynamic;

namespace CMSSolutions.Dynamic
{
    public class TrustDynamicObject : DynamicObject
    {
        private readonly IDictionary<string, object> values;

        public TrustDynamicObject()
        {
            values = new Dictionary<string, object>();
        }

        public object this[string name]
        {
            get
            {
                if (values.ContainsKey(name))
                {
                    return values[name];
                }
                return string.Empty;
            }
            set { values[name] = value; }
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var key = indexes[0].ToString();
            values[key] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = values.ContainsKey(binder.Name) ? values[binder.Name] : string.Empty;
            return true;
        }
    }
}