using System;
using System.Collections.Generic;
using System.Dynamic;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class DynamicControlObject : DynamicObject
    {
        private readonly IDictionary<string, object> values;

        public DynamicControlObject()
        {
            values = new Dictionary<string, object>();
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            values[binder.Name] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (values.ContainsKey(binder.Name))
            {
                result = values[binder.Name];
                return true;
            }

            result = null;
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var key = Convert.ToString(indexes[0]);
            if (values.ContainsKey(key))
            {
                result = values[key];
                return true;
            }
            result = null;
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var key = Convert.ToString(indexes[0]);
            values[key] = value;
            return true;
        }
    }
}